using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs
{
    public record CreateUserDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; init; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; init; }

        [Required(ErrorMessage = "CPF or Passport is required.")]
        public string CPF_Passport { get; init; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; init; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; init; }
    }
}
