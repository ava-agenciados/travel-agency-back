using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Requests.Media;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Utils; // Adicionado para usar DashboardMetricsPdfService
using travel_agency_back.Models;

namespace travel_agency_back.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    public class DashBoardController : Controller
    {

        private readonly IAdminService _adminService;
        private readonly IMediaService _mediaService;
        public DashBoardController(IAdminService adminService, IMediaService mediaService)
        {
            _adminService = adminService;
            _mediaService = mediaService;
        }
        /// <summary>
        /// Cria um novo pacote turístico.
        /// </summary>
        /// <remarks>Endpoint protegido, apenas administradores podem criar pacotes.</remarks>
        [SwaggerOperation(
            Summary = "Cria um novo pacote turístico",
            Description = "Endpoint protegido, administradores criam pacotes já disponíveis, atendentes criam pacotes pendentes de aprovação."
        )]
        [HttpPost("packages/create")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> CreateNewPackage([FromBody] CreateNewPackageDTO packageRequestDTO)
        {
            if (packageRequestDTO == null)
            {
                return BadRequest(new GenericResponseDTO(400, "Dados inválidos", false));
            }
            // Verifica o papel do usuário autenticado
            if (User.IsInRole("Admin"))
            {
                packageRequestDTO.IsAvailable = true;
            }
            else if (User.IsInRole("Atendente"))
            {
                packageRequestDTO.IsAvailable = false;
            }
            var response = await _adminService.CreatePackageAsync(packageRequestDTO);
            if (response == null)
            {
                return BadRequest(new GenericResponseDTO(500, "Erro ao criar pacote", false));
            }
            return Ok(new GenericResponseDTO(200, "Pacote criado com sucesso!.", true));
        }


        /// <summary>
        /// Atualiza um pacote turístico pelo ID.
        /// </summary>
        /// <remarks>Endpoint protegido, administradores e atendentes podem atualizar pacotes.</remarks>
        [SwaggerOperation(
            Summary = "Atualiza um pacote turístico pelo ID",
            Description = "Endpoint protegido, administradores e atendentes podem atualizar pacotes."
        )]
        [HttpPatch("packages/update/{packageId}")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> UpdatePackage(int packageId, [FromBody] UpdatePackageDTO dto)
        {
            var result = await _adminService.UpdatePackageByIdAsync(packageId, dto);
            if (result is NotFoundObjectResult)
                return NotFound();
            return Ok(new GenericResponseDTO(200, "Pacote atualizado com sucesso!", true));
        }

        /// <summary>
        /// Retorna todos os pacotes turísticos disponíveis.
        /// </summary>
        /// <remarks>Endpoint protegido, administradores e atendentes podem visualizar todos os pacotes.</remarks>
        [SwaggerOperation(
            Summary = "Retorna todos os pacotes turísticos disponíveis",
            Description = "Endpoint protegido, administradores e atendentes podem visualizar todos os pacotes."
        )]
        [HttpGet("packages/all")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetAllPackages()
        {
            var response = await _adminService.GetAllPackagesAsync();
            return Ok(response);
        }

        /// <summary>
        /// Retorna um pacote turístico pelo ID.
        /// </summary>
        /// <remarks>Endpoint protegido, administradores e atendentes podem visualizar o pacote.</remarks>
        [SwaggerOperation(
            Summary = "Retorna um pacote turístico pelo ID",
            Description = "Endpoint protegido, administradores e atendentes podem visualizar o pacote."
        )]
        [HttpGet("packages/{packageId}")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetPackageById(int packageId)
        {
            var response = await _adminService.GetPackageByIdAsync(packageId);
            if (response == null)
                return NotFound();
            return Ok(response);
        }
        [SwaggerOperation(
            Summary = "Adiciona uma nova ou varias midias a um pacote",
            Description = "Endpoint protegido, administradores e atendentes podem enviar midias para um pacote"
        )]
        [HttpPost("packages/{packageId}/add-midia")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> UploadMediaToPackage(int packageId, [FromForm] List<IFormFile> media)
        {
            var result = await _mediaService.UploadMediaAsync(packageId, media);
            if (result is BadRequestObjectResult)
            {
                return BadRequest(new GenericResponseDTO(400, "Formato de midia não autorizado", false));
            }
            if (result is NotFoundObjectResult)
            {
                return NotFound(new GenericResponseDTO(404, "Pacote não encontrado", false));
            }
            return Ok(new GenericResponseDTO(200, "Midia enviada com sucesso!", true));
        }

        [SwaggerOperation(
            Summary = "Retorna todas as mídias de um pacote pelo ID",
            Description = "Endpoint protegido, administradores e atendentes podem visualizar as mídias do pacote."
        )]
        [HttpGet("packages/{packageId}/media")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetMediaByPackageId(int packageId)
        {
            var result = await _mediaService.GetPackageMediaAsync(packageId);
            return result;
        }

        /// <summary>
        /// Deleta um pacote turístico pelo ID.
        /// </summary>
        /// <remarks>Endpoint protegido, apenas administradores podem deletar pacotes.</remarks>
        [SwaggerOperation(
            Summary = "Deleta um pacote turístico pelo ID",
            Description = "Endpoint protegido, apenas administradores podem deletar pacotes."
        )]
        [HttpDelete("packages/delete/{packageID}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage(int packageID)
        {
            if (packageID <= 0)
            {
                return BadRequest(new GenericResponseDTO(404, "ID do pacote inválido", false));
            }
            var response = await _adminService.DeletePackageAsync(packageID);
            if (response == null)
            {
                return NotFound(new GenericResponseDTO(404, "Pacote não encontrado", false));
            }
            return Ok(new GenericResponseDTO(200, "Pacote deletado com sucesso!", true));
        }

        [HttpDelete("packages/{packageId}/media/{mediaName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMediaFromPackage(int packageId, string mediaName)
        {
            var result = await _mediaService.DeleteMediaFromPackageAsync(packageId, mediaName);
            return result;
        }

        [SwaggerOperation(
            Summary = "Deleta uma avaliação pelo ID",
            Description = "Endpoint protegido, apenas administradores podem deletar avaliações."
        )]
        [HttpDelete("packages/ratings/{ratingId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRating(int ratingId)
        {
            if (ratingId <= 0)
            {
                return BadRequest(new GenericResponseDTO(404, "ID da avaliação inválido", false));
            }
            var response = await _adminService.DeleteRating(ratingId);
            if (response == null)
            {
                return NotFound(new GenericResponseDTO(404, "Avaliação não encontrada", false));
            }
            return Ok(new GenericResponseDTO(200, "Avaliação deletada com sucesso!", true));

        }

        [SwaggerOperation(
            Summary = "Altera o cargo de um usuário",
            Description = "Endpoint protegido, apenas administradores podem alterar o cargo de um usuário."
        )]
        [HttpPatch("users/change-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDTO dto)
        {
            var result = await _adminService.ChangeUserRoleAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [SwaggerOperation(
            Summary = "Retorna métricas de vendas (por destino, período, cliente)",
            Description = "Endpoint protegido, apenas administradores e atendentes podem visualizar as métricas de vendas."
        )]
        [HttpGet("metrics")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetSalesMetrics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var metrics = await _adminService.GetSalesMetricsAsync(startDate, endDate);
            return Ok(metrics);
        }

        [SwaggerOperation(
            Summary = "Gera um relatório PDF das métricas de vendas",
            Description = "Endpoint protegido, apenas administradores podem baixar o relatório em PDF. O layout segue o padrão dos e-mails."
        )]
        [HttpGet("metrics/pdf")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadMetricsPdf([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var metrics = await _adminService.GetSalesMetricsAsync(startDate, endDate);
            var pdfBytes = DashboardMetricsPdfService.GenerateMetricsReport(metrics);
            var fileName = $"relatorio-metricas-{DateTime.Now:yyyyMMdd-HHmm}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        [SwaggerOperation(
            Summary = "Retorna todas as reservas com detalhes (admin/atendente)",
            Description = "Endpoint protegido, retorna todas as reservas com informações completas de pagamento, acompanhantes, status, etc. O front pode filtrar por usuário ou outros critérios."
        )]
        [HttpGet("bookings")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _adminService.GetAllBookingsDetailedAsync();
            return Ok(bookings);
        }

        [SwaggerOperation(
            Summary = "Atualiza uma reserva pelo ID",
            Description = "Endpoint protegido, apenas administradores e atendentes podem atualizar reservas."
        )]
        [HttpPatch("bookings/{bookingId}")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] Booking updatedBooking)
        {
            var result = await _adminService.UpdateBookingAsync(bookingId, updatedBooking);
            if (result is NotFoundObjectResult)
                return NotFound();
            return Ok(new GenericResponseDTO(200, "Reserva atualizada com sucesso!", true));
        }

        [SwaggerOperation(
            Summary = "Deleta uma reserva pelo ID",
            Description = "Endpoint protegido, apenas administradores podem deletar reservas."
        )]
        [HttpDelete("bookings/{bookingId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var result = await _adminService.DeleteBookingAsync(bookingId);
            if (result is NotFoundObjectResult)
                return NotFound(new GenericResponseDTO(404, "Reserva não encontrada", false));
            return Ok(new GenericResponseDTO(200, "Reserva deletada com sucesso!", true));
        }
    }
}
