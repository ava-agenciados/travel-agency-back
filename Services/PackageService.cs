using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly UserManager<User> _userManager;
        public PackageService(IPackageRepository packageRepository, UserManager<User> userManager)
        {
            _packageRepository = packageRepository;
            _userManager = userManager;
        }

        public async Task<GenericResponseDTO> AddComment(int packageID, string email, string comment, int rating)
        {
           var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return new GenericResponseDTO(404, "Usuário não encontrado", false);
            }

            var package = await _packageRepository.GetPackageByIdAsync(packageID);
            if (package == null)
            {
                return new GenericResponseDTO(404, "Pacote não encontrado", false);
            }
            if (rating < 1 || rating > 5)
            {
                return new GenericResponseDTO(400, "A avaliação deve ser entre 1 e 5", false);
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                return new GenericResponseDTO(400, "O comentário não pode ser vazio", false);
            }
            var ratingEntity = new Rating
            {
                Stars = rating,
                Comment = comment,
                UserId = user.Id,
                PackageId = package.Id,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            // Adiciona o comentário e a avaliação ao pacote
            var result = await _packageRepository.AddComment(ratingEntity);
            
            if (result == null)
            {
                return new GenericResponseDTO(500, "Erro ao adicionar comentário", false);
            }
            return new GenericResponseDTO(200, "Comentário adicionado com sucesso", true);
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
                Ratings = p.Ratings?
                    .Where(r => r.IsAvailable)
                    .Select(r => new PackageRatingResponseDTO
                    {
                        Id = r.Id,
                        Rating = r.Stars,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt,
                        UserName = r.User != null
                            ? (!string.IsNullOrEmpty(r.User.Email)
                                ? r.User.Email
                                : $"{r.User.Email}")
                            : null
                    }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList(),
                DiscountPercent = p.DiscountPercent,
                LodgingInfo = p.LodgingInfo == null ? null : new DTOs.Packages.LodgingInfoDTO
                {
                    Baths = p.LodgingInfo.Baths,
                    Beds = p.LodgingInfo.Beds,
                    WifiIncluded = p.LodgingInfo.WifiIncluded,
                    ParkingSpot = p.LodgingInfo.ParkingSpot,
                    SwimmingPool = p.LodgingInfo.SwimmingPool,
                    FitnessCenter = p.LodgingInfo.FitnessCenter,
                    RestaurantOnSite = p.LodgingInfo.RestaurantOnSite,
                    PetAllowed = p.LodgingInfo.PetAllowed,
                    AirConditioned = p.LodgingInfo.AirConditioned,
                    Breakfast = p.LodgingInfo.Breakfast,
                    Location = new DTOs.Packages.AddressDTO
                    {
                        Street = p.LodgingInfo.Street,
                        Number = p.LodgingInfo.Number,
                        Neighborhood = p.LodgingInfo.Neighborhood,
                        City = p.LodgingInfo.City,
                        State = p.LodgingInfo.State,
                        Country = p.LodgingInfo.Country,
                        ZipCode = p.LodgingInfo.ZipCode,
                        Complement = p.LodgingInfo.Complement
                    }
                }
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
                Ratings = p.Ratings?.Where(r => r.IsAvailable).Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Stars,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserName = r.User != null ? ($"{r.User.FirstName} {r.User.LastName}") : string.Empty,
                    UserEmail = r.User != null ? r.User.Email : string.Empty
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList(),
                DiscountPercent = p.DiscountPercent ?? 0, // valor padrão 0 se nulo
                LodgingInfo = p.LodgingInfo == null ? new DTOs.Packages.LodgingInfoDTO() : new DTOs.Packages.LodgingInfoDTO
                {
                    Baths = p.LodgingInfo.Baths,
                    Beds = p.LodgingInfo.Beds,
                    WifiIncluded = p.LodgingInfo.WifiIncluded,
                    ParkingSpot = p.LodgingInfo.ParkingSpot,
                    SwimmingPool = p.LodgingInfo.SwimmingPool,
                    FitnessCenter = p.LodgingInfo.FitnessCenter,
                    RestaurantOnSite = p.LodgingInfo.RestaurantOnSite,
                    PetAllowed = p.LodgingInfo.PetAllowed,
                    AirConditioned = p.LodgingInfo.AirConditioned,
                    Breakfast = p.LodgingInfo.Breakfast,
                    Location = new DTOs.Packages.AddressDTO
                    {
                        Street = p.LodgingInfo.Street,
                        Number = p.LodgingInfo.Number,
                        Neighborhood = p.LodgingInfo.Neighborhood,
                        City = p.LodgingInfo.City,
                        State = p.LodgingInfo.State,
                        Country = p.LodgingInfo.Country,
                        ZipCode = p.LodgingInfo.ZipCode,
                        Complement = p.LodgingInfo.Complement
                    }
                }
            }).ToList();
            return Task.FromResult(response);
        }
    }
}
