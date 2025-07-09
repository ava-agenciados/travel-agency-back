namespace travel_agency_back.Models
{
    public class AvaliablePackages
    {
        public string PackageName { get; set; } = string.Empty;
        public string Destination{ get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public decimal Price     { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
                          
        public DateTime DepartureDate{ get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
