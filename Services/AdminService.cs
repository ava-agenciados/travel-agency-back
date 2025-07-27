using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.DTOs.Responses.Dashboard;

namespace travel_agency_back.Services
{
    public class AdminService : IAdminService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<User> _userManager;
        public AdminService(IPackageRepository packageRepository, IBookingRepository bookingRepository, UserManager<User> userManager)
        {
            _packageRepository = packageRepository;
            _bookingRepository = bookingRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> CreatePackageAsync(CreateNewPackageDTO createNewPackageDTO)
        {
            LodgingInfo lodgingInfo = null;
            if (createNewPackageDTO.LodgingInfo != null)
            {
                lodgingInfo = new LodgingInfo
                {
                    Baths = createNewPackageDTO.LodgingInfo.Baths ?? 0,
                    Beds = createNewPackageDTO.LodgingInfo.Beds ?? 0,
                    WifiIncluded = createNewPackageDTO.LodgingInfo.WifiIncluded ?? false,
                    ParkingSpot = createNewPackageDTO.LodgingInfo.ParkingSpot ?? false,
                    SwimmingPool = createNewPackageDTO.LodgingInfo.SwimmingPool ?? false,
                    FitnessCenter = createNewPackageDTO.LodgingInfo.FitnessCenter ?? false,
                    RestaurantOnSite = createNewPackageDTO.LodgingInfo.RestaurantOnSite ?? false,
                    PetAllowed = createNewPackageDTO.LodgingInfo.PetAllowed ?? false,
                    AirConditioned = createNewPackageDTO.LodgingInfo.AirConditioned ?? false,
                    Breakfast = createNewPackageDTO.LodgingInfo.Breakfast ?? false,
                    Street = createNewPackageDTO.LodgingInfo.Location?.Street,
                    Number = createNewPackageDTO.LodgingInfo.Location?.Number,
                    Neighborhood = createNewPackageDTO.LodgingInfo.Location?.Neighborhood,
                    City = createNewPackageDTO.LodgingInfo.Location?.City,
                    State = createNewPackageDTO.LodgingInfo.Location?.State,
                    Country = createNewPackageDTO.LodgingInfo.Location?.Country,
                    ZipCode = createNewPackageDTO.LodgingInfo.Location?.ZipCode,
                    Complement = createNewPackageDTO.LodgingInfo.Location?.Complement
                };
            }

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
                ImageUrl = createNewPackageDTO.ImageUrl,
                Bookings = new List<Booking>(),
                Ratings = new List<Rating>(),
                PackageMedia = createNewPackageDTO.PackageMedia?.Select(pm => new PackageMedia { ImageURL = pm.ImageURL, MediaType = pm.MediaType }).ToList(),
                CreatedAt = DateTime.UtcNow,
                LodgingInfo = lodgingInfo,
                DiscountPercent = createNewPackageDTO.DiscountPercent
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
                    Comment = r.Comment,
                    UserName = r.User?.UserName,
                    UserEmail = r.User?.Email,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList(),
                DiscountPercent = p.DiscountPercent,
                LodgingInfo = p.LodgingInfo == null ? null : new travel_agency_back.DTOs.Packages.LodgingInfoDTO
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
                    Location = new travel_agency_back.DTOs.Packages.AddressDTO
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
                    Comment = r.Comment,
                    UserName = r.User?.UserName,
                    UserEmail = r.User?.Email,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.ImageURL
                }).ToList(),
                DiscountPercent = p.DiscountPercent,
                LodgingInfo = p.LodgingInfo == null ? null : new travel_agency_back.DTOs.Packages.LodgingInfoDTO
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
                    Location = new travel_agency_back.DTOs.Packages.AddressDTO
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

            // Atualiza apenas os campos enviados
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

            // Atualiza LodgingInfo se enviado
            if (dto.LodgingInfo != null)
            {
                if (package.LodgingInfo == null)
                {
                    package.LodgingInfo = new LodgingInfo();
                }
                if (dto.LodgingInfo.Baths.HasValue) package.LodgingInfo.Baths = dto.LodgingInfo.Baths.Value;
                if (dto.LodgingInfo.Beds.HasValue) package.LodgingInfo.Beds = dto.LodgingInfo.Beds.Value;
                if (dto.LodgingInfo.WifiIncluded.HasValue) package.LodgingInfo.WifiIncluded = dto.LodgingInfo.WifiIncluded.Value;
                if (dto.LodgingInfo.ParkingSpot.HasValue) package.LodgingInfo.ParkingSpot = dto.LodgingInfo.ParkingSpot.Value;
                if (dto.LodgingInfo.SwimmingPool.HasValue) package.LodgingInfo.SwimmingPool = dto.LodgingInfo.SwimmingPool.Value;
                if (dto.LodgingInfo.FitnessCenter.HasValue) package.LodgingInfo.FitnessCenter = dto.LodgingInfo.FitnessCenter.Value;
                if (dto.LodgingInfo.RestaurantOnSite.HasValue) package.LodgingInfo.RestaurantOnSite = dto.LodgingInfo.RestaurantOnSite.Value;
                if (dto.LodgingInfo.PetAllowed.HasValue) package.LodgingInfo.PetAllowed = dto.LodgingInfo.PetAllowed.Value;
                if (dto.LodgingInfo.AirConditioned.HasValue) package.LodgingInfo.AirConditioned = dto.LodgingInfo.AirConditioned.Value;
                if (dto.LodgingInfo.Breakfast.HasValue) package.LodgingInfo.Breakfast = dto.LodgingInfo.Breakfast.Value;
                if (dto.LodgingInfo.Location != null)
                {
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.Street)) package.LodgingInfo.Street = dto.LodgingInfo.Location.Street;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.Number)) package.LodgingInfo.Number = dto.LodgingInfo.Location.Number;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.Neighborhood)) package.LodgingInfo.Neighborhood = dto.LodgingInfo.Location.Neighborhood;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.City)) package.LodgingInfo.City = dto.LodgingInfo.Location.City;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.State)) package.LodgingInfo.State = dto.LodgingInfo.Location.State;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.Country)) package.LodgingInfo.Country = dto.LodgingInfo.Location.Country;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.ZipCode)) package.LodgingInfo.ZipCode = dto.LodgingInfo.Location.ZipCode;
                    if (!string.IsNullOrEmpty(dto.LodgingInfo.Location.Complement)) package.LodgingInfo.Complement = dto.LodgingInfo.Location.Complement;
                }
                // Marcar LodgingInfo como modificado
                _packageRepository.MarkLodgingInfoAsModified(package.LodgingInfo);
            }

            await _packageRepository.SaveChangesAsync();
            return new OkObjectResult(new { Message = "Alterações salvas com sucesso." });
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
        public async Task<GenericResponseDTO> DeleteRating(int ratingId)
        {
            var rating = await _packageRepository.DeleteRating(ratingId);
            if (rating == null)
            {
                return new GenericResponseDTO(404, "Comentário não encontrado", false);

            }
            return new GenericResponseDTO(200, "Comentário excluído com sucesso", true);
        }

        public async Task<GenericResponseDTO> ChangeUserRoleAsync(ChangeUserRoleDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new GenericResponseDTO(404, "Usuário não encontrado", false);

            var validRoles = new[] { "Admin", "Atendente", "Cliente" };
            if (!validRoles.Contains(dto.NewRole))
                return new GenericResponseDTO(400, "Cargo inválido", false);

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var removeErrors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return new GenericResponseDTO(500, $"Erro ao remover cargos antigos: {removeErrors}", false);
            }

            var addResult = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!addResult.Succeeded)
            {
                var addErrors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return new GenericResponseDTO(500, $"Erro ao adicionar novo cargo: {addErrors}", false);
            }

            // Atualiza a propriedade Role do usuário e salva no banco
            user.Role = dto.NewRole;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var updateErrors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return new GenericResponseDTO(500, $"Erro ao atualizar o campo Role: {updateErrors}", false);
            }

            return new GenericResponseDTO(200, "Cargo alterado com sucesso", true);
        }

        public async Task<SalesMetricsDTO> GetSalesMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _bookingRepository.GetSalesMetricsAsync(startDate, endDate);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllBookingsDetailedAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return bookings.Select(b => new BookingResponseDTO
            {
                Id = b.Id,
                PackageId = b.PackageId,
                TravelDate = b.TravelDate,
                Status = b.Status,
                Companion = b.Companions?.Select(c => new CompanionResponseDTO
                {
                    FullName = c.FullName,
                    DocumentNumber = c.DocumentNumber
                }).ToList() ?? new List<CompanionResponseDTO>(),
                Payment = b.Payments?.Select(p => new PaymentResponseDTO
                {
                    PaymentMethod = p.PaymentMethod
                }).ToList() ?? new List<PaymentResponseDTO>(),
                LodgingInfo = b.Package?.LodgingInfo == null ? null : new travel_agency_back.DTOs.Packages.LodgingInfoDTO
                {
                    Baths = b.Package.LodgingInfo.Baths,
                    Beds = b.Package.LodgingInfo.Beds,
                    WifiIncluded = b.Package.LodgingInfo.WifiIncluded,
                    ParkingSpot = b.Package.LodgingInfo.ParkingSpot,
                    SwimmingPool = b.Package.LodgingInfo.SwimmingPool,
                    FitnessCenter = b.Package.LodgingInfo.FitnessCenter,
                    RestaurantOnSite = b.Package.LodgingInfo.RestaurantOnSite,
                    PetAllowed = b.Package.LodgingInfo.PetAllowed,
                    AirConditioned = b.Package.LodgingInfo.AirConditioned,
                    Breakfast = b.Package.LodgingInfo.Breakfast,
                    Location = new travel_agency_back.DTOs.Packages.AddressDTO
                    {
                        Street = b.Package.LodgingInfo.Street,
                        Number = b.Package.LodgingInfo.Number,
                        Neighborhood = b.Package.LodgingInfo.Neighborhood,
                        City = b.Package.LodgingInfo.City,
                        State = b.Package.LodgingInfo.State,
                        Country = b.Package.LodgingInfo.Country,
                        ZipCode = b.Package.LodgingInfo.ZipCode,
                        Complement = b.Package.LodgingInfo.Complement
                    }
                },
                DiscountPercent = b.Package?.DiscountPercent,
                ContractingUser = b.User == null ? null : new UserSummaryDTO
                {
                    Id = b.User.Id,
                    FirstName = b.User.FirstName,
                    LastName = b.User.LastName,
                    Email = b.User.Email,
                    CPFPassport = b.User.CPFPassport
                }
            });
        }

        public async Task<IActionResult> UpdateBookingAsync(int bookingId, Booking updatedBooking)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return new NotFoundObjectResult(new { Message = "Reserva não encontrada" });

            // Atualiza os campos permitidos
            booking.TravelDate = updatedBooking.TravelDate;
            booking.Status = updatedBooking.Status;
            booking.UpdatedAt = DateTime.UtcNow;
            // Outros campos podem ser atualizados conforme a regra de negócio

            await _bookingRepository.UpdateUserBookingAsync(booking.UserId, booking);
            return new OkObjectResult(new { Message = "Reserva atualizada com sucesso." });
        }

        public async Task<IActionResult> DeleteBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return new NotFoundObjectResult(new { Message = "Reserva não encontrada" });

            await _bookingRepository.DeleteUserBookingAsync(booking.UserId, bookingId);
            return new OkObjectResult(new { Message = "Reserva deletada com sucesso." });
        }
    }
}
