using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests.Media
{
    public class UploadMediaDTO
    {
        [Required]
        public IFormFile File { get; set; } // Arquivo de mídia a ser enviado
        [Required]
        public int PackageId { get; set; } // ID do pacote associado à mídia
        [Required]
        public int MediaType { get; set; } // Tipo de mídia (1 para imagem, 2 para vídeo)
    }
}
