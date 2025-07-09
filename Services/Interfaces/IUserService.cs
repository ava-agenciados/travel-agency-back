using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;

namespace travel_agency_back.Services.Interfaces
{
    public interface IUserService
    {
        public bool UserEmailExists(string email);
        public bool UserCpfPassportExists(string cpfPassport);
    }
}
