namespace travel_agency_back.Models
{
    public class Rating
    {
        public int Id { get; set; } // Identificador único da avaliação
        public int UserId { get; set; } // Chave estrangeira para o usuário que fez a avaliação
        public User User { get; set; } // Navegação para o usuário associado
        public int PackageId { get; set; } // Chave estrangeira para o pacote avaliado
        public Packages Package { get; set; } // Navegação para o pacote avaliado
        public int Stars { get; set; } // Avaliação em estrelas (1 a 5)
        public string Comment { get; set; } = string.Empty; // Comentário da avaliação
        public bool IsAvailable { get; set; } = true; // Indica se a avaliação está ativa ou não
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora da criação da avaliação
    }
}
