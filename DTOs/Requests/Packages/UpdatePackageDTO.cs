namespace travel_agency_back.DTOs.Requests.Packages
{
    public class UpdatePackageDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveUntil { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Quantity { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public DTOs.Packages.LodgingInfoDTO? LodgingInfo { get; set; } // Permite atualizar LodgingInfo
    }
}