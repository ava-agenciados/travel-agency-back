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

        public async Task<User> GetUserByCPFPassportAsync(string CPFPassport)
        {
            var user = await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.CPFPassport == CPFPassport);
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var result = await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if(result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return result;
        }

    }
}
