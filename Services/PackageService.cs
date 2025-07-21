using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync()
        {
            var result = await _packageRepository.GetAllPackagesAsync();
            if (result == null)
                return Enumerable.Empty<PackageResponseDTO>();

            var response = result.Select(p => new PackageResponseDTO
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
            return response;
        }

        public Task<PackageResponseDTO> GetPackageByIdAsync(int packageID)
        {
            throw new NotImplementedException();
        }

        public Task<List<PackageResponseDTO>> GetPackagesByFilterAsync(string? origin, string? destination, DateTime? startDate, DateTime? endDate)
        {
            var result = _packageRepository.GetPackagesByFilter(origin, destination, startDate, endDate);
            if (result == null)
            {
                return Task.FromResult(new List<PackageResponseDTO>());
            }
            var response = result.Result.Select(p => new PackageResponseDTO
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
            }).ToList();
            return Task.FromResult(response);
        }
    }
}
