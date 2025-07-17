using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Models;

namespace travel_agency_back.Services.Interfaces
{
    public interface IBookingService
    {
        public Task<IActionResult> CreateUserBookingAsync(int userId, string userEmail, int packageId, ICollection<string> payment, CreateNewBookingDTO createNewBooking);
        public Task<IActionResult> GetUserBookingsAsync(int userId);
    }
}
