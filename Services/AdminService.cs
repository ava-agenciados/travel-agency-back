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
                }).ToList() ?? new List<PaymentResponseDTO>()
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
