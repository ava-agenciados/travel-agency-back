using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;

namespace travel_agency_back.Services.Interfaces
{
    public interface IPackageService
    {
       public Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync();
       public Task<PackageResponseDTO> GetPackageByIdAsync(int packageID);
        public Task<List<PackageResponseDTO>> GetPackagesByFilterAsync(string? origin, string? destination, DateTime? startDate, DateTime? endDate);
        public Task<GenericResponseDTO> AddComment(int packageID, string email, string comment, int rating);

    }
}
