using Microsoft.AspNetCore.Identity;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Models;

namespace travel_agency_back.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password);
        public Task<GenericResponseDTO> RegisterAsync(string firstname, string lastname, string email, string phonenumber, string CPFPassport, string password);
        public Task<bool> Logout(string token);
        Task GetUserByEmailAsync(string email);

        public Task<(IdentityResult, string URL, bool emailSent)> GeneratePasswordResetTokenAsync(string email);
        public Task<GenericResponseDTO> ResetPasswordAsync(string token, string email, string newPassword);
    }

}
