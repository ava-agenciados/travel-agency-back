namespace travel_agency_back.DTOs.Resposes.Packages
{
    public class PackageDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Price { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveUntil { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }//
        public string ImageUrl { get; set; } // Assuming ImageUrl is a string
        public DateTime CreatedAt { get; set; }
    }
}
