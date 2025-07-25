namespace travel_agency_back.DTOs.Resposes.Packages
{
    public class PackageMediaResponseDTO
    {
        public int? Id { get; set; }
        public int? MediaType { get; set; } // 1 = Imagem, 2 = Vídeo
        public string? MediaUrl { get; set; } // URL do arquivo de mídia//
    }
}