namespace travel_agency_back.DTOs.Resposes
{
    public class BookingResponseDTO
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public DateTime TravelDate { get; set; }
        public string Status { get; set; }
        public List<CompanionResponseDTO> Companion { get; set; } = new ();
        public List<PaymentResponseDTO> Payment { get; set; }
        public travel_agency_back.DTOs.Packages.LodgingInfoDTO? LodgingInfo { get; set; } // NOVO: info de acomodação do pacote
        public double? DiscountPercent { get; set; } // NOVO: desconto do pacote
        public UserSummaryDTO? ContractingUser { get; set; } // NOVO: informações do contratante
        // NOVO: opcionais escolhidos
        public bool HasTravelInsurance { get; set; }
        public bool HasTourGuide { get; set; }
        public bool HasTour { get; set; }
        public bool HasActivities { get; set; }
    }

    public class UserSummaryDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CPFPassport { get; set; }
    }
}