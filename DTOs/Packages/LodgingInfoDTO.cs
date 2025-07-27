namespace travel_agency_back.DTOs.Packages
{
    public class LodgingInfoDTO
    {
        // Removido LodgingId, pois o Id é gerado automaticamente pelo banco
        public int? Baths { get; set; }
        public int? Beds { get; set; }
        public bool? WifiIncluded { get; set; } = false;
        public bool? ParkingSpot { get; set; } = false;
        public bool? SwimmingPool { get; set; } = false;
        public bool? FitnessCenter { get; set; } = false;
        public bool? RestaurantOnSite { get; set; } = false;
        public bool? PetAllowed { get; set; } = false;
        public bool? AirConditioned { get; set; } = false;
        public bool? Breakfast { get; set; } = false;
        public AddressDTO? Location { get; set; }
    }
}