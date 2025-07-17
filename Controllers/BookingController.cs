using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPackageRepository _packageRepository;

        private readonly IBookingService _bookingService;

        public BookingController(IBookingRepository bookingRepository, IPackageRepository packageRepository, IBookingService bookingService)
        {
            _bookingRepository = bookingRepository;
            _packageRepository = packageRepository;
            _bookingService = bookingService;
        }

        //Retorna todos as reservas do usuario autenticado
        [HttpGet("bookings")]
        [Authorize]
        public Task<IActionResult> GetAllBookings()
        {
           
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Task.FromResult<IActionResult>(Unauthorized(new GenericResponseDTO(401, "Acesso não autorizado", false)));
            }
             
            int userId = int.Parse(userIdClaim.Value);
            var bookings = _bookingRepository.GetUserBookingsAsync(userId).Result;
            if (bookings == null || !bookings.Any())
                return Task.FromResult<IActionResult>(NotFound(new GenericResponseDTO(404, "Nenhum pacote encontrado", false)));

            var response = bookings.Select(b => new BookingResponseDTO
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

            return Task.FromResult<IActionResult>(Ok(response));
        }

        [HttpPut("bookings/payment")]
        [Authorize]
        public async Task<IActionResult> CreateNewBooking([FromBody] CreateNewBookingDTO createNewBooking)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userEmailClaim = User.FindFirst(ClaimTypes.Email); 
            if (userIdClaim == null)
                return Unauthorized();
            if (createNewBooking == null)
                return BadRequest("Dados de reserva inválidos.");
            int userId = int.Parse(userIdClaim.Value);

            // Lógica do status da reserva baseado no método de pagamento principal
            var mainPayment = createNewBooking.PaymentMethods?.FirstOrDefault();
            string status = "Pendente";
            if (mainPayment != null)
            {
                switch (mainPayment.PaymentMethod)
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
            var paymentMethods = createNewBooking.PaymentMethods?.Select(p => p.PaymentMethod.ToString()).ToList() ?? new List<string>();

            var result = await _bookingService.CreateUserBookingAsync(userId, userEmailClaim.Value, createNewBooking.PackageID, paymentMethods, booking);
            var package = await _packageRepository.GetPackageByIdAsync(createNewBooking.PackageID);
            if (result is OkObjectResult)
            {
                await EmailService.SendPaymentConfirmation(User.Claims, createNewBooking, paymentMethods, package);
                return Ok(new GenericResponseDTO(200, "Reserva criada com sucesso", true));
            }
            return BadRequest(new GenericResponseDTO(500, "Falha ao cria reserva", false));
        }

        //Rotas Admin e Atendente

        //Retorna todas as reservas de TODOS os usuários
        [HttpGet("dashboard/bookings")]
        [Authorize(Roles = "Admin,Atendente")]
        public IActionResult GetBookings()
        {
            return Ok(new { Message = "Lista de todas as reservas" });
        }
    }
}
