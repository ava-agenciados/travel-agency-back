using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.DTOs.Responses.User;
using System.Security.Claims;

namespace travel_agency_back.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retorna as informações do usuário autenticado.
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();
            var dto = new UserProfileResponseDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                CPFPassport = user.CPFPassport,
                Role = user.Role
            };
            return Ok(dto);
        }
    }
}
