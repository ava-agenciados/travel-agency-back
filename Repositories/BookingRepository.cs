using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace travel_agency_back.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private ApplicationDBContext _context;
        public BookingRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Implements the required interface method
        public async Task<IActionResult> CreateUserBookingAsync(int userId, string userEmail, int packageId, ICollection<string> payment, Booking booking)
        {
            if (booking == null)
                return new BadRequestObjectResult("Booking inválido.");

            booking.UserId = userId;
            booking.PackageId = packageId;
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return new OkObjectResult(booking);
        }

        public Task DeleteUserBookingAsync(int userId, int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetUserBookingByIdAsync(int userId, int bookingId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Companions)
                .Include(b => b.Payments)
                .ToListAsync();
            return bookings;
        }

        public Task UpdateUserBookingAsync(int userId, Booking booking)
        {
            throw new NotImplementedException();
        }

        // Implements the required interface method
        public Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            throw new NotImplementedException();
        }
    }
}
