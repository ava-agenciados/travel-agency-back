using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;
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

        public AuthService(IUserService userService)
        {
            _userService = userService;
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

        public async Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string phonenumber, string CPFPassport, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = email,
                PhoneNumber = phonenumber,
                CPFPassport = CPFPassport
            };
            return await _userService.RegisterAsync(
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.CPFPassport,
                password
            );
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
