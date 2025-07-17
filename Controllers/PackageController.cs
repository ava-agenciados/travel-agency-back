using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        /*
         
                    ROTAS ADMIN  E ATENDENTE
         
         */

        //Rotas Admin, Atendente
        //Cria um novo pacote
        [HttpPost("dashboard/packages/create")]
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

        //Deleta um pacote específico pelo ID
        [HttpPost("dashboard/packages/delete/{packageID}")]
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

        //Atualiza um pacote específico pelo ID
        [HttpPut("dashboard/packages/update/{packageId}")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> UpdatePackage(int packageId, [FromBody] UpdatePackageDTO dto)
        {
            var result = await _adminService.UpdatePackageByIdAsync(packageId, dto);
            if (result is NotFoundObjectResult)
                return NotFound();
            return Ok(new GenericResponseDTO(200, "Pacote atualizado com sucesso!", true));
        }

        //Retorna todos os pacotes disponíveis
        [HttpGet("dashboard/packages/all")]
        [Authorize(Roles = "Admin, Atendente")]
        public async Task<IActionResult> GetAllPackages()
        {
            var response = await _adminService.GetAllPackagesAsync();
            return Ok(response);
        }

        //Retorna um pacote específico pelo ID
        [HttpGet("dashboard/packages/{packageId}")]
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
