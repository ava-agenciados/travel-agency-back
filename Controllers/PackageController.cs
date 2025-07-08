using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs;

namespace travel_agency_back.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        [HttpGet("packages/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> PackageSearch([FromQuery] PackageSearchDTO packageSearchDTO)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> GetMyPackages()
        {
            //TODO: Retorna uma lista de todos os pacotes do usuário autenticado
            return Task.FromResult<IActionResult>(Ok("Return all user packages"));
        }
    }
}
