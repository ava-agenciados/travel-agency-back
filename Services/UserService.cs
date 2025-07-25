using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;
using travel_agency_back.Utils;
using System.Security.Claims;

namespace travel_agency_back.Services
{
    public class UserService : IUserService
    {
        //autenticação de usuário
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        //repositório de usuário
        private readonly IUserRepository _userRepository;


        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository userRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string phonenumber, string CPFPassport, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                CPFPassport = CPFPassport,
                UserName = email
            };
            if(string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phonenumber) || string.IsNullOrEmpty(CPFPassport) || string.IsNullOrEmpty(password))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Os campos precisam está preenchidos" });
            }
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "E-mail já está em uso" });
            }
            var userCPF =  await _userRepository.GetUserByCPFPassportAsync(CPFPassport);
            if (userCPF != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "CPF ou número de passaporte já está em uso"});
            }

            var newUserResult = await _userManager.CreateAsync(user, password);
            if (!newUserResult.Succeeded)
            {
                // Retorne os erros detalhados do Identity
                return newUserResult;
            }
            // Adiciona a role ao usuário
            await _userManager.AddToRoleAsync(user, user.Role);
            return IdentityResult.Success;
        }

        public async Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (SignInResult.Failed, null);
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Gera o token JWT com a role
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? user.Role;
                var token = JWTGenerationToken.GenerateToken(user.Id.ToString(), email, role, _configuration);
                return (SignInResult.Success, token);
            }
            return (SignInResult.Failed, null);
        }

        public async Task<SignInResult> ResetPasswordAsync(string token, string email, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return SignInResult.Failed;
            }
           
            var decodedToken = System.Net.WebUtility.UrlDecode(token);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;

            }

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (!resetResult.Succeeded)
            {
                return SignInResult.Failed;
            }
            return SignInResult.Success;
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            var user = _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<(IdentityResult, string URL, bool emailSent)> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return (IdentityResult.Failed(), null, false);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                return (IdentityResult.Failed(), null, false);
            }

            var baseUrl = "https://localhost:7283";
            var resetUrl = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            //envia email com o link de redefinição de senha
            EmailService.SendPasswordResetEmail(
                email: user.Email,
                username: user.FirstName,
                linkReset: resetUrl
            );
            return (IdentityResult.Success, resetUrl, true);
        }

        public async Task<IEnumerable<Booking>> GetAllUserBookingsAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            var bookings = await _userRepository.GetUserBookingsAsync(user.Id);
            if(bookings == null || !bookings.Any())
            {
                throw new Exception("Nenhuma reserva encontrada para o usuário");
            }
            return bookings;
        }

        public Task<User> GetUserByCPFPassport(string cpfPassport)
        {
            var user = _userRepository.GetUserByCPFPassportAsync(cpfPassport);
            return user;
        }
    }
}
