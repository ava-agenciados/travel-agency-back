using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;

namespace travel_agency_back.Services.Interfaces
{
    public interface IAuthService
    {
        // Novo método para login com retorno de token JWT
        Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password);
        public Task<SignInResult> LoginAsync(string email, string password);
        public Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string CPFPassport, string password);
        public Task<User> FindByEmailAsync(string email);
        public Task<string> GeneratePasswordResetTokenAsync(User user);
        public Task<SignInResult> ResetPasswordAsync(User user, string token, string newPassword);
    }

}
