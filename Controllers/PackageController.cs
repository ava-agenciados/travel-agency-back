using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento e consulta de pacotes turísticos na API.
    /// 
    /// Esta classe expõe endpoints para:
    /// - Buscar pacotes disponíveis com filtros opcionais (destino, origem, data de partida) via DTO.
    /// - Listar todos os pacotes disponíveis para compra.
    /// - Listar todos os pacotes adquiridos pelo usuário autenticado.
    /// 
    /// Funcionalidades principais:
    /// - GET /api/v1/packages/search: Permite buscar pacotes disponíveis utilizando parâmetros de consulta encapsulados no <see cref="PackageSearchRequestDTO"/>.
    /// - GET /api/v1/packages/avaliable: Retorna todos os pacotes disponíveis no sistema.
    /// - GET /api/v1/packages/my-packages: Retorna todos os pacotes adquiridos pelo usuário autenticado.
    /// 
    /// Observações:
    /// - Os métodos retornam respostas HTTP apropriadas para cada cenário.
    /// - A autenticação pode ser exigida em métodos específicos, conforme a necessidade de negócio.
    /// - A implementação dos métodos deve ser feita para acessar os dados reais dos pacotes e usuários.
    /// </summary>
    /// 
    [Route("api/v1")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IUserService _userService;


        public PackageController(IPackageService packageService, IUserService userService)
        {
            _packageService = packageService;
            _userService = userService;
        }

        [HttpGet("packages/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> PackageSearch([FromQuery] PackageSearchRequestDTO packageSearchDTO)
        {
            //TODO: Retorna uma lista de todos os pacotes disponiveis
            return Task.FromResult<IActionResult>(Ok("All packages retrieved successfully"));
        }

        [HttpGet("packages/avaliable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> GetAllAvaliablePackages()
        {
            //TODO: Retorna uma lista de todos os pacotes disponiveis
            return Task.FromResult<IActionResult>(Ok("All packages retrieved successfully"));
        }

        [HttpGet("packages/my-packages")]
        [Authorize]
        public async Task<IActionResult> GetMyPackages()
        {
            var userPackages = await _userService.GetAllUserBookingsAsync(User);

            if(userPackages == null)
            {
                return NotFound("No packages found for the user.");
            }
            var userPackagesResponse = new UserPackagesResponse
            {
                packages = userPackages.Select(p => new PackageResponseDTO
                {
                    Id = p.Id,
                    Destination = p.Package.Destination,
                    Origin = p.Package.Origin,
                    DepartureDate = p.BookingDate,
                    Price = p.Package.Price,
                    Description = p.Package.Description,
                }).ToList()
            };
            return Ok(userPackagesResponse);
        }
    }
}
