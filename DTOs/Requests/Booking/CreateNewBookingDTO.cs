namespace travel_agency_back.DTOs.Requests.Booking
{
    public class CreateNewBookingDTO
    {
        public int PackageID { get; set; }

        public DateTime StartTravel { get; set; }
        public DateTime EndTravel { get; set; }
        public IEnumerable<CompanionsRequestDTO> Companions { get; set; } = new List<CompanionsRequestDTO>();
        public IEnumerable<PaymentDTO> PaymentMethods { get; set; } = new List<PaymentDTO>();
    }
}
