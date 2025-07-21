using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using travel_agency_back.Models;

namespace travel_agency_back.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string phonenumber, string CPFPassport, string password);
        public Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password);
        public Task<SignInResult> ResetPasswordAsync(string token, string email, string newPassword);

        public Task<User> GetUserByEmailAsync(string email);

        public Task<(IdentityResult, string URL, bool emailSent)> GeneratePasswordResetTokenAsync(string email);


        public Task<IEnumerable<Booking>> GetAllUserBookingsAsync(ClaimsPrincipal principal);

    }
}
