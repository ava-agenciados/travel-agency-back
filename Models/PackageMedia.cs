namespace travel_agency_back.Models
{
    public class PackageMedia
    {
        public int Id { get; set; }
        public int PackageId { get; set; } // Chave estrangeira para o pacote associado
        public Packages Package { get; set; } // Navegação para o pacote associado
        public int MediaType { get; set; } // Tipo de mídia (1 para imagem, 2 para vídeo)
        public string ImageURL { get; set; } // URL da imagem do pacote
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora de criação da mídia
    }
}
