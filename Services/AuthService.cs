using Microsoft.AspNetCore.Identity;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Services
{
    /// <summary>
    /// Serviço de autenticação responsável apenas por login e geração de token JWT.
    /// Toda lógica de usuário (registro, reset, etc) deve estar no UserService.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public AuthService(IUserService userService, IUserRepository userRepository, UserManager<User> userManager)
        {
            _userService = userService;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password)
        {
            var (result, token) = await _userService.LoginWithTokenAsync(email, password);
            if (result == null && string.IsNullOrEmpty(token))
            {
                return (SignInResult.Failed, null);
            }
            return (result, token);
        }

        public async Task<GenericResponseDTO> RegisterAsync(string firstname, string lastname, string email, string phonenumber, string CPFPassport, string password)
        {
            if (string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phonenumber) || string.IsNullOrEmpty(CPFPassport) || string.IsNullOrEmpty(password))
            {
                return new GenericResponseDTO(400, "Os campos precisam está preenchidos", false);
            }
            if(string.IsNullOrEmpty(phonenumber) || phonenumber.Length < 10)
            {
                return new GenericResponseDTO(400, "O número de telefone deve ter pelo menos 10 dígitos", false);
            }
            if(CPFPassport.Length < 6 || CPFPassport.Length > 11)
            {
                return new GenericResponseDTO(400, "O CPF ou número de passaporte deve ter entre 6 e 11 caracteres", false);
            }
            // Verifica se já existe um usuário com o mesmo e-mail
            var existingEmail = await _userService.GetUserByEmailAsync(email);
            if (existingEmail != null)
            {
                return new GenericResponseDTO(401, "Já existe um usuário com este e-mail", false);
            }
            var existingCPFPassport = await _userRepository.GetUserByCPFPassportAsync(CPFPassport);
            if (existingCPFPassport != null)
            {
                return new GenericResponseDTO(401, "Já existe um usuário com este CPF ou número de passaporte", false);
            }
            // Cria o usuário com os dados fornecidos
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = email,
                PhoneNumber = phonenumber,
                CPFPassport = CPFPassport
            };
            var result = await _userManager.CreateAsync(user,password);
            if (!result.Succeeded)
            {
                return new GenericResponseDTO(400, "A senha deve conter pelo menos 9 caracteres, incluindo ao menos uma letra maiúscula, um número e um caractere especial.", false);
            }
            return new GenericResponseDTO(200, "Usuario registrado com sucesso", true);
        }

        public Task<bool> Logout(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<(IdentityResult, string URL, bool emailSent)> GeneratePasswordResetTokenAsync(string email)
        {
            var (user, URL, emailSent) = await _userService.GeneratePasswordResetTokenAsync(email);

            if (user == null)
            {
                return (IdentityResult.Failed(), null, false);
            }
            if (string.IsNullOrEmpty(URL))
            {
                return (IdentityResult.Failed(), null, false);
            }
          
            return (IdentityResult.Success, URL, emailSent);
        }

        public Task GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> ResetPasswordAsync(string token, string email, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return IdentityResult.Failed();
            }

            var resetResult = await _userService.ResetPasswordAsync(token, email, newPassword);
            if (resetResult == null || !resetResult.Succeeded)
            {
                return IdentityResult.Failed();
            }
            return IdentityResult.Success;
        }
    }
}
