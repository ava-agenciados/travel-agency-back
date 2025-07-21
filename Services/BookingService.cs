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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository bookingRepository, IPackageRepository packageRepository, IUserRepository userRepository, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _packageRepository = packageRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IActionResult> CreateUserBookingAsync(int userId, CreateNewBookingDTO createNewBooking)
        {
            var paymentMethods = createNewBooking.PaymentMethods?.Select(p => p.PaymentMethod.ToString()).ToList() ?? new List<string>();
            var payment = createNewBooking.PaymentMethods?.FirstOrDefault();
            var userInfo = await _userRepository.GetUserByIdAsync(userId);


            var booking = new Booking
            {
                PackageId = createNewBooking.PackageID,
                TravelDate = createNewBooking.StartTravel,
                Status = "",
                Companions = createNewBooking.Companions?.Select(c => new Companions
                {
                    FullName = $"{c.FirstName} {c.LastName}",
                    DocumentNumber = c.CPFPassport,
                }).ToList() ?? new List<Companions>(),
                Payments = createNewBooking.PaymentMethods?.Select(p => new Payments
                {
                    PaymentMethod = p.PaymentMethod.ToString(),
                    PaymentDate = DateTime.UtcNow,

                }).ToList() ?? new List<Payments>()
            };

            var package = await _packageRepository.GetPackageByIdAsync(booking.PackageId);
            if (package == null)
            {
               
                return new NotFoundObjectResult(new GenericResponseDTO(404, "Pacote não encontrado", false));
            }
            if (userInfo == null)
            {

                return new NotFoundObjectResult(new GenericResponseDTO(404, "Usuário não encontrado", false));
            }
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
           
            switch (payment.PaymentMethod)
            {
                case PaymentMethod.Pix:
                    booking.Status = "Aprovado";
                    booking.Payments.FirstOrDefault().Status = "Aprovado";
                    await EmailService.SendPixPaymentConfirmation(userInfo, createNewBooking.PaymentMethods?.FirstOrDefault(), package, booking, _logger);
                    break;
                case PaymentMethod.Boleto:
                    booking.Status = "Pendente";
                    booking.Payments.FirstOrDefault().Status = "Pendente";
                    EmailService.SendBoletoEmail(
                        userInfo.Email,
                        userInfo.FirstName,
                        userInfo.LastName,
                        userInfo.CPFPassport,
                        package.Price,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        booking.TravelDate // ou outro campo de fim
                    );
                    break;
                case PaymentMethod.CreditCard:
                    booking.Status = "Recusado";
                    booking.Payments.FirstOrDefault().Status = "Recusado";
                    EmailService.SendCartaoEmail(
                        userInfo.Email,
                        payment.FirstName,
                        payment.LastName,
                        payment.CPFPassport,
                        package.Price,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        booking.TravelDate, // ou outro campo de fim
                        payment.Installments, // parcelas, ajuste conforme necessário
                        "Recusado", // status, ajuste conforme necessário
                        payment.TransactionId.ToString()
                    );
                    break;
                case PaymentMethod.DebitCard:
                    booking.Status = "Recusado";
                    booking.Payments.FirstOrDefault().Status = "Recusado";
                    EmailService.SendCartaoEmail(
                        userInfo.Email,
                        payment.FirstName,
                        payment.LastName,
                        payment.CPFPassport,
                        package.Price,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        booking.TravelDate, // ou outro campo de fim
                        1, // parcelas, ajuste conforme necessário
                        "Recusado", // status, ajuste conforme necessário
                        payment.TransactionId.ToString()
                    );
                    break;
                default:
                    _logger.LogWarning("Método de pagamento não reconhecido para envio de e-mail.");
                    break;
            }
            var result = await _bookingRepository.CreateUserBookingAsync(userId, booking.PackageId, booking);
            
            if (result is OkObjectResult && payment != null)
            {
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
