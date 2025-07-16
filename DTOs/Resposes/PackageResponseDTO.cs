namespace travel_agency_back.DTOs.Resposes
{
    public class PackageResponseDTO
    {
        /// <summary>
        /// Identificador único do pacote.
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Nome do pacote turístico.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Descrição detalhada do pacote.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Preço do pacote.
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// Data de partida do pacote.
        /// </summary>
        public DateTime? DepartureDate { get; set; }
        /// <summary>
        /// Data de retorno do pacote.
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        public string? Origin { get; set; }
        /// <summary>
        /// Destino do pacote turístico.
        /// </summary>
        public string? Destination { get; set; }
    }
}
