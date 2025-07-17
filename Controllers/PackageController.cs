using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Requests.Packages;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
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
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;


        public PackageController(IPackageService packageService, IAdminService adminService, IUserService userService)
        {
            _packageService = packageService;
            _adminService = adminService;
            _userService = userService;
        }
        /// <summary>
        /// Retorna todos os pacotes disponíveis.
        /// </summary>
        /// <remarks>Endpoint para listar pacotes disponíveis para compra.</remarks>
        [SwaggerOperation(
            Summary = "Retorna todos os pacotes disponíveis - Rota livre sem autenticação",
            Description = "Endpoint para listar pacotes disponíveis para compra."
        )]
        [HttpGet("packages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAvaliablePackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            if(packages == null)
            {
                return BadRequest(new GenericResponseDTO(404, "Nenhum pacote encontrado", false));
            }
            var response = packages.Select(p => new PackageResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DepartureDate = p.DepartureDate,
                ReturnDate = p.ReturnDate,
                Origin = p.Origin,
                Destination = p.Destination,
                IsActive = p.IsActive,
                Ratings = p.Ratings?.Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.MediaUrl
                }).ToList()
            });
            return Ok(response);
        }
        [SwaggerOperation(
            Summary = "Retorna um pacote com base no seu ID - Rota livre sem autenticação",
            Description = "Endpoint para retornar pacote por seu respectivo ID."
        )]
        [HttpGet("packages/{packageId}")]
        public async Task<IActionResult> GetPackageByIdUser(int packageId)
        {
            var package = await _adminService.GetPackageByIdAsync(packageId);
            if (package == null)
                return NotFound(new GenericResponseDTO(404, "Pacote não encontrado", false));
            
            var response = new PackageResponseDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                DepartureDate = package.DepartureDate,
                ReturnDate = package.ReturnDate,
                Origin = package.Origin,
                Destination = package.Destination,
                IsActive = package.IsActive,
                Ratings = package.Ratings?.Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList(),
                PackageMedia = package.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.MediaUrl
                }).ToList()
            };
            return Ok(response);
        }
        [SwaggerOperation(
            Summary = "Retorna que retorna pacotes com base em seu filtro de pesquisa(origem, destino, datas) - Rota livre sem autenticação",
            Description = "Endpoint para retornar pacotes com base em filtros"
        )]
        [HttpGet("packages/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PackageSearch([FromQuery] PackageSearchRequestDTO packageSearchDTO)
        {
            
             var packages = await _packageService.GetPackagesByFilterAsync(
                packageSearchDTO.Origin,
                packageSearchDTO.Destination,
                packageSearchDTO.DepartureDate,
                packageSearchDTO.ReturnDate
            );
            if (packages == null || !packages.Any())
            {
                return BadRequest(new GenericResponseDTO(404, "Nenhum pacote encontrado", false));
            }
            var response = packages.Select(p => new PackageResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DepartureDate = p.DepartureDate,
                ReturnDate = p.ReturnDate,
                Origin = p.Origin,
                Destination = p.Destination,
                IsActive = p.IsActive,
                Ratings = p.Ratings?.Select(r => new PackageRatingResponseDTO
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment
                }).ToList(),
                PackageMedia = p.PackageMedia?.Select(pm => new PackageMediaResponseDTO
                {
                    Id = pm.Id,
                    MediaType = pm.MediaType,
                    MediaUrl = pm.MediaUrl
                }).ToList()
            });
            return Ok(response);
        }
    }
}
