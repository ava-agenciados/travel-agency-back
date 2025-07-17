using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Data;
using travel_agency_back.Models;
using Microsoft.EntityFrameworkCore;

namespace travel_agency_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext? _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            var userBookings = await _context.UserBookings
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Package)
            .ToListAsync();
            return userBookings;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var result = await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return result;
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
