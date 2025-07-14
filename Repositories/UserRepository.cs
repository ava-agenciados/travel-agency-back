using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using travel_agency_back.Data;

namespace travel_agency_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext? _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool UserCPFPassportExists(string CPFPassport)
        {
            var CPFPassportExists = _context.Users.Any(u => u.CPFPassport == CPFPassport);
            if(CPFPassportExists)
            {
                return true;
            }
            return false;
        }
    }
}
