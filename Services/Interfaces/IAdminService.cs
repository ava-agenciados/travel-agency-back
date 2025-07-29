using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.DTOs.Responses.Dashboard;

namespace travel_agency_back.Services.Interfaces
{
    public interface IAdminService
    {
        public Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync();
        public Task<PackageResponseDTO> GetPackageByIdAsync(int packageID);
        public Task<IActionResult> CreatePackageAsync(CreateNewPackageDTO createNewPackageDTO);
        public Task<IActionResult> UpdatePackageAsync(int packageID, Packages package);
        public Task<IActionResult> DeletePackageAsync(int packageID);
        public Task<GenericResponseDTO> DeleteRating(int ratingId);
        Task<IActionResult> UpdatePackageByIdAsync(int packageId, UpdatePackageDTO dto);
        Task<GenericResponseDTO> ChangeUserRoleAsync(ChangeUserRoleDTO dto);
        Task<SalesMetricsDTO> GetSalesMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<BookingResponseDTO>> GetAllBookingsDetailedAsync();
        Task<IActionResult> UpdateBookingAsync(int bookingId, Booking updatedBooking);
        Task<IActionResult> DeleteBookingAsync(int bookingId);
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    }
}
