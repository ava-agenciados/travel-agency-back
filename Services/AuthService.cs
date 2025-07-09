using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{

    ///<summary>
    ///
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string CPFPassport, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = email,
                CPFPassport = CPFPassport
            };
            var result = await _userManager.CreateAsync(user, password);
            if(!result.Succeeded)
            {
               await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return result;
        }
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {

            return await _userManager.GeneratePasswordResetTokenAsync(user);
            
        }

        public Task<SignInResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            if(user == null || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return Task.FromResult(SignInResult.Failed);
            }
            var result = _userManager.ResetPasswordAsync(user, token, newPassword);

            if(!result.Result.Succeeded)
            {
                foreach (var error in result.Result.Errors)
                {
                    Console.WriteLine($"[Reset Error] {error.Code} - {error.Description}");
                }
            }

            return Task.FromResult(SignInResult.Success);
        }
    }
}
