using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Responses.Dashboard;
using travel_agency_back.Models;

namespace travel_agency_back.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        //Funções GET
        public Task<List<Booking>> GetUserBookingsAsync(int userId);
        public Task<Booking> GetBookingByIdAsync(int bookingId);
        public Task<List<Booking>> GetAllBookingsAsync();
        public Task<Booking> GetUserBookingByIdAsync(int userId, int bookingId);

        public Task<List<Booking>> GetBookingsByPackageIdAsync(int packageId);
        public Task<List<Booking>> GetActiveBookingsAsync();
        public Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
        public Task<List<Booking>> GetBookingsByStatusAsync(string status);
        public Task<List<Booking>> GetBookingsByTravelDateAsync(DateTime travelDate);
        public Task<List<Booking>> GetBookingsByBookingDateAsync(DateTime bookingDate);

        //Funções Set/Create
        public Task<IActionResult> CreateUserBookingAsync(int userId, int packageId, Booking booking);
        public Task UpdateUserBookingAsync(int userId, Booking booking);
        public Task DeleteUserBookingAsync(int userId, int bookingId);
        public Task<SalesMetricsDTO> GetSalesMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
