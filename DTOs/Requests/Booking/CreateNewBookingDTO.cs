namespace travel_agency_back.DTOs.Requests.Booking
{
    public class CreateNewBookingDTO
    {
        public int PackageID { get; set; }

        public DateTime StartTravel { get; set; }
        public DateTime EndTravel { get; set; }
        public List<CompanionsRequestDTO> Companions { get; set; } = new();
        public List<PaymentDTO> PaymentMethods { get; set; } = new();
    }
}
