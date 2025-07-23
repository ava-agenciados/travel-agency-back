namespace travel_agency_back.DTOs.Resposes
{
    internal class BookingResponseDTO
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public DateTime TravelDate { get; set; }
        public string Status { get; set; }
        public List<CompanionResponseDTO> Companion { get; set; } = new ();
        public List<PaymentResponseDTO> Payment { get; set; }
    }
}