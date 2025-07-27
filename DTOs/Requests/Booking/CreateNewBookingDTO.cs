namespace travel_agency_back.DTOs.Requests.Booking
{
    public class CreateNewBookingDTO
    {
        public int PackageID { get; set; }

        public DateTime StartTravel { get; set; }
        public DateTime EndTravel { get; set; }
        public IEnumerable<CompanionsRequestDTO> Companions { get; set; } = new List<CompanionsRequestDTO>();
        public IEnumerable<PaymentDTO> PaymentMethods { get; set; } = new List<PaymentDTO>();
        // Opcionais
        public bool? HasTravelInsurance { get; set; }
        public bool? HasTourGuide { get; set; }
        public bool? HasTour { get; set; }
        public bool? HasActivities { get; set; }
    }
}
