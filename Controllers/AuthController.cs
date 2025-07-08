using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs;

namespace travel_agency_back.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("auth/register")]
        public Task<IActionResult> Register([FromBody]CreateUserDTO userDTO)
        {
            //TODO implementar a lógica de registro
            return Task.FromResult<IActionResult>(Ok("Register successful"));
        }


        [HttpPost]
        [Route("auth/login")]
        public Task<IActionResult> Login([FromBody]LoginUserDTO userDTO)
        {
            // TODO implementar a lógica de autenticação
            return Task.FromResult<IActionResult>(Ok("Login successful"));
        }
    }
}
