using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace travel_agency_back.DTOs
{
    public record LoginUserDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        
        public string Password { get; init; }
    }
}
