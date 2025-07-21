using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    public class AdminService : IAdminService
    {
        private readonly IPackageRepository _packageRepository;
        public AdminService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<IActionResult> CreatePackageAsync(CreateNewPackageDTO createNewPackageDTO)
        {
            var package = new Packages
            {
                Name = createNewPackageDTO.Name,
                Description = createNewPackageDTO.Description,
                Origin = createNewPackageDTO.Origin,
                Destination = createNewPackageDTO.Destination,
                Price = createNewPackageDTO.Price,
                ActiveFrom = createNewPackageDTO.ActiveFrom,
                ActiveUntil = createNewPackageDTO.ActiveUntil,
                BeginDate = createNewPackageDTO.BeginDate,
                EndDate = createNewPackageDTO.EndDate,
                Quantity = createNewPackageDTO.Quantity,
                
                IsAvailable = createNewPackageDTO.IsAvailable,
                ImageUrl = createNewPackageDTO.ImageUrl, // Assuming ImageUrl is part of CreateNewPackageDTO
                Bookings = new List<Booking>(),
                Ratings = new List<Rating>(),
                PackageMedia = createNewPackageDTO.PackageMedia?.Select(pm => new PackageMedia { ImageURL = pm.ImageURL, MediaType = pm.MediaType }).ToList(),
                CreatedAt = DateTime.UtcNow
            };
            var result = await _packageRepository.CreateNewPackageAsync(package);
            return result;
        }

        public async Task<IActionResult> DeletePackageAsync(int packageID)
        {
            var package = await _packageRepository.GetPackageByIdAsync(packageID);
            if (package == null)
            {
                return new NotFoundObjectResult(new { Message = "Package not found" });
            }
            var result = await _packageRepository.DeletePackageByIdAsync(packageID);
            return result;
        }

        public async Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _packageRepository.GetAllPackagesAsync();
            return packages.Select(p => new PackageResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DepartureDate = p.BeginDate,
                ReturnDate = p.EndDate,
                Origin = p.Origin,
                Destination = p.Destination,
                IsActive = p.IsAvailable,
                Ratings = p.Ratings?.Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Stars,
                    Comment = r.Comment
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList()
            });
        }

        public async Task<PackageResponseDTO> GetPackageByIdAsync(int packageID)
        {
            var p = await _packageRepository.GetPackageByIdAsync(packageID);
            if (p == null) return null;
            return new PackageResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DepartureDate = p.BeginDate,
                ReturnDate = p.EndDate,
                Origin = p.Origin,
                Destination = p.Destination,
                IsActive = p.IsAvailable,
                Ratings = p.Ratings?.Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Stars,
                    Comment = r.Comment
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList()
            };
        }

        public Task<IActionResult> UpdatePackageAsync(int packageID, Packages package)
        {
            var existingPackage = _packageRepository.GetPackageByIdAsync(packageID);
            if (existingPackage == null)
            {
                return Task.FromResult<IActionResult>(new NotFoundObjectResult(new { Message = "Package not found" }));
            }
            return _packageRepository.UpdatePackageByIdAsync(packageID, package);

        }

        public async Task<IActionResult> UpdatePackageByIdAsync(int packageID, UpdatePackageDTO dto)
        {
            var package = await _packageRepository.GetPackageByIdAsync(packageID);
            if (package == null)
                return new NotFoundObjectResult(new { Message = "Package not found" });

            if (dto.Name != null) package.Name = dto.Name;
            if (dto.Description != null) package.Description = dto.Description;
            if (dto.Price.HasValue) package.Price = dto.Price.Value;
            if (dto.Origin != null) package.Origin = dto.Origin;
            if (dto.Destination != null) package.Destination = dto.Destination;
            if (dto.ActiveFrom.HasValue) package.ActiveFrom = dto.ActiveFrom.Value;
            if (dto.ActiveUntil.HasValue) package.ActiveUntil = dto.ActiveUntil.Value;
            if (dto.BeginDate.HasValue) package.BeginDate = dto.BeginDate.Value;
            if (dto.EndDate.HasValue) package.EndDate = dto.EndDate.Value;
            if (dto.Quantity.HasValue) package.Quantity = dto.Quantity.Value;
            if (dto.IsAvailable.HasValue) package.IsAvailable = dto.IsAvailable.Value;
            if (dto.ImageUrl != null) package.ImageUrl = dto.ImageUrl;

            var result = await _packageRepository.UpdatePackageByIdAsync(packageID, package);
            return result;
        }

        async Task<IActionResult> IAdminService.UpdatePackageByIdAsync(int packageId, UpdatePackageDTO dto)
        {
            var package = await _packageRepository.GetPackageByIdAsync(packageId);
            if (package == null)
                return new NotFoundObjectResult(new { Message = "Package not found" });

            if (dto.Name != null) package.Name = dto.Name;
            if (dto.Description != null) package.Description = dto.Description;
            if (dto.Price.HasValue) package.Price = dto.Price.Value;
            if (dto.Origin != null) package.Origin = dto.Origin;
            if (dto.Destination != null) package.Destination = dto.Destination;
            if (dto.ActiveFrom.HasValue) package.ActiveFrom = dto.ActiveFrom.Value;
            if (dto.ActiveUntil.HasValue) package.ActiveUntil = dto.ActiveUntil.Value;
            if (dto.BeginDate.HasValue) package.BeginDate = dto.BeginDate.Value;
            if (dto.EndDate.HasValue) package.EndDate = dto.EndDate.Value;
            if (dto.Quantity.HasValue) package.Quantity = dto.Quantity.Value;
            if (dto.IsAvailable.HasValue) package.IsAvailable = dto.IsAvailable.Value;
            if (dto.ImageUrl != null) package.ImageUrl = dto.ImageUrl;

            var result = await _packageRepository.UpdatePackageByIdAsync(packageId, package);
            return new OkObjectResult(new { Message = "Alterações salvas com sucesso." });
        }
    }
}
