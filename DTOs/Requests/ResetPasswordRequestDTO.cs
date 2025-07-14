using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        public string Password { get; set; }
    }
}
