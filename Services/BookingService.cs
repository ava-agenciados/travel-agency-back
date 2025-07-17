using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPackageRepository _packageRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IActionResult> CreateUserBookingAsync(int userId, string userEmail, int packageId, ICollection<string> payment, CreateNewBookingDTO createNewBooking)
        { 
            var paymentMethod = createNewBooking.PaymentMethods?.FirstOrDefault();
            var status = "Pendente";
            switch (paymentMethod.PaymentMethod)
            {
                case PaymentMethod.CreditCard:
                case PaymentMethod.DebitCard:
                    status = "Recusado";
              
                    break;
                case PaymentMethod.Boleto:
                    status = "Pendente";
                    break;
                case PaymentMethod.Pix:
                    status = "Confirmado";
                    break;
            }
            var booking = new Booking
            {
                PackageId = createNewBooking.PackageID,
                TravelDate = createNewBooking.StartTravel,
                Status = status,
                Companions = createNewBooking.Companions?.Select(c => new Companions
                {
                    FullName = $"{c.FirstName} {c.LastName}",
                    DocumentNumber = c.CPFPassport,
                }).ToList() ?? new List<Companions>(),
                Payments = createNewBooking.PaymentMethods?.Select(p => new Payments
                {
                    PaymentMethod = p.PaymentMethod.ToString(),
                }).ToList() ?? new List<Payments>()
            };
            var package = await  _packageRepository.GetPackageByIdAsync(packageId);

            var packageResponse = new PackageDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Origin = package.Origin,
                Destination = package.Destination,
                Price = package.Price,
                ActiveFrom = package.ActiveFrom,
                ActiveUntil = package.ActiveUntil,
                BeginDate = package.BeginDate,
                EndDate = package.EndDate,
                Quantity = package.Quantity,
                IsAvailable = package.IsAvailable,
                ImageUrl = package.ImageUrl,
            };

            var result = await _bookingRepository.CreateUserBookingAsync(userId, packageId, payment, booking);
            if (result is OkObjectResult)
            { 
                await EmailService.SendPaymentConfirmation()
                return new OkObjectResult(new GenericResponseDTO(200, "Reserva criada com sucesso", true));
            }
            return new ObjectResult(new GenericResponseDTO(500, "Falha ao gerar reserva de viagem", false));
        }

        public Task<IActionResult> GetUserBookingsAsync(int userId)
        {
            var bookings = _bookingRepository.GetUserBookingsAsync(userId);
            if (bookings == null || !bookings.Result.Any())
            {
                return Task.FromResult<IActionResult>(new NotFoundObjectResult(new GenericResponseDTO(404, "Nenhuma reserva encontrada", false)));
            }
            var response = bookings.Result.Select(b => new BookingResponseDTO
            {
                Id = b.Id,
                PackageId = b.PackageId,
                TravelDate = b.TravelDate,
                Status = b.Status,
                Companion = b.Companions?.Select(c => new CompanionResponseDTO
                {
                    FullName = c.FullName,
                    DocumentNumber = c.DocumentNumber
                }).ToList(),
                Payment = b.Payments?.Select(p => new PaymentResponseDTO
                {
                    PaymentMethod = p.PaymentMethod
                }).ToList()
            }).ToList();

            return Task.FromResult<IActionResult>(new OkObjectResult(response));
        }
    }
}
