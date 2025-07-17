using Microsoft.AspNetCore.Mvc;
using travel_agency_back.Models;

namespace travel_agency_back.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        //Funções GET
        public Task<List<Booking>> GetUserBookingsAsync(int userId);
        public Task<Booking> GetBookingByIdAsync(int bookingId);
        public Task<Booking> GetUserBookingByIdAsync(int userId, int bookingId);

        //Funções Set/Create
        public Task<IActionResult> CreateUserBookingAsync(int userId, int packageId, Booking booking);
        public Task UpdateUserBookingAsync(int userId, Booking booking);
        public Task DeleteUserBookingAsync(int userId, int bookingId);
    }
}
