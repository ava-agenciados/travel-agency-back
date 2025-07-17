using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests.Booking
{
    public class CompanionsRequestDTO
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? CPFPassport { get; set; }
        [Required]
        public bool? IsForeigner { get; set; } = false; // Indica se o acompanhante é estrangeiro
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; } // Corrigido: Nome correto do campo
    }
}
