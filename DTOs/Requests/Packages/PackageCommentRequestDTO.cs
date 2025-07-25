using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests.Packages
{
    public class PackageCommentRequestDTO
    {
        [Required]
        public string Email { get; set; } // Email do usuário que está comentando   
        [Required]
        public string? Comment { get; set; }
        [Required]
        public int Rating { get; set; } // Avaliação do pacote, de 1 a 5
    }
}
