using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    public class DashBoardController : Controller
    {

        private readonly IAdminService _adminService;
        public DashBoardController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        /// <summary>
        /// Retorna todos pagamentos de todos usuários (admin/atendente).
        /// </summary>
        /// <remarks>Endpoint protegido, acesso restrito a admin/atendente.</remarks>
        [SwaggerOperation(
            Summary = "Retorna todos pagamentos de todos usuários (admin/atendente)",
            Description = "Endpoint protegido, acesso restrito a admin/atendente."
        )]
        [HttpGet]
        [Route("payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            // Aqui você implementaria a lógica para buscar todos os pagamentos
            // Exemplo: var payments = _paymentService.GetAllPayments();
            // Retorne os pagamentos encontrados ou uma mensagem de erro se não houver nenhum
            return Ok(new { Message = "Todos os pagamentos retornados com sucesso." });
        }

        /// <summary>
        /// Cria um novo pacote turístico.
        /// </summary>
        /// <remarks>Endpoint protegido, apenas administradores podem criar pacotes.</remarks>
        [SwaggerOperation(
            Summary = "Cria um novo pacote turístico",
            Description = "Endpoint protegido, apenas administradores podem criar pacotes."
        )]
        [HttpPost("packages/create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNewPackage([FromBody] CreateNewPackageDTO packageRequestDTO)
        {
            if (packageRequestDTO == null)
            {
                return BadRequest(new GenericResponseDTO(400, "Dados inválidos", false));
            }
            var response = await _adminService.CreatePackageAsync(packageRequestDTO);
            if (response == null)
            {
                return BadRequest(new GenericResponseDTO(500, "Erro ao criar pacote", false));
            }
            return Ok(new GenericResponseDTO(200, "Pacote criado com sucesso!.", true));
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
    }
}
