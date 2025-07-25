using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        /// <summary>
        /// Retorna todas as reservas do usuário autenticado.
        /// </summary>
        /// <remarks>Endpoint protegido, retorna reservas do usuário logado.</remarks>
        [SwaggerOperation(
            Summary = "Retorna todas as reservas do usuário autenticado",
            Description = "Endpoint protegido, retorna reservas do usuário logado."
        )]
        //Retorna todos as reservas do usuario autenticado
        [HttpGet("bookings")]
        [Authorize]
        public Task<IActionResult> GetAllBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
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

        /// <summary>
        /// Cria uma nova reserva para o usuário autenticado.
        /// </summary>
        /// <remarks>Endpoint protegido, realiza a reserva e pagamento.</remarks>
        [SwaggerOperation(
            Summary = "Cria uma nova reserva para o usuário autenticado",
            Description = "Endpoint protegido, realiza a reserva e pagamento."
        )]
        [HttpPut("bookings/payment")]
        [Authorize]
        public async Task<IActionResult> CreateNewBooking([FromBody] CreateNewBookingDTO createNewBooking)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            if (createNewBooking == null)
                return BadRequest("Dados de reserva inválidos.");
            int userId = int.Parse(userIdClaim.Value);

            var result = await _bookingService.CreateUserBookingAsync(userId, createNewBooking);
          
            if (result is OkObjectResult)
            {
                return Ok(new GenericResponseDTO(200, "Reserva criada com sucesso", true));
            }
            return BadRequest(new GenericResponseDTO(500, "Falha ao cria reserva", false));
        }
    }
}
