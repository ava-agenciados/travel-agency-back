namespace travel_agency_back.DTOs.Resposes.Packages
{
    public class PackageRatingResponseDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserName { get; set; } // Nome do usuário que fez a avaliação
        public string? UserEmail { get; set; } // Email do usuário que fez a avaliação
        public DateTime CreatedAt { get; set; } // Data e hora em que a avaliação foi criada
    }
}