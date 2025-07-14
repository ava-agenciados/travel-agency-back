using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;
using travel_agency_back.Utils;

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

        public Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string CPFPassport, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                CPFPassport = CPFPassport,
                UserName = email // Define o UserName como o email
            };

            if(_userRepository.UserCPFPassportExists(CPFPassport))
            {
                // Se o CPF/Passaporte já existir, retorne um erro
                return Task.FromResult(IdentityResult.Failed());
            }

    
            var newUser = _userManager.CreateAsync(user, password);

            if (newUser.IsFaulted)
            {
                // Se ocorrer um erro, retorne o resultado com o erro
                return Task.FromResult(IdentityResult.Failed());
            }
            return newUser;
        }
        public async Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password)
        {
            // busco o usuário pelo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (SignInResult.Failed, null);
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Gera o token JWT aqui
                var token = JWTGenerationToken.GenerateToken(user.Id, email, _configuration);
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
            if (user.IsFaulted || user.Result == null)
            {
                throw new Exception("Usuário não encontrado");
            }
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
    }
}
