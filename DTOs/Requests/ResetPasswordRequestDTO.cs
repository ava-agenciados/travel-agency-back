using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
    }
}
