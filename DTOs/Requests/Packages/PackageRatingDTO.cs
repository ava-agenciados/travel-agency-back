namespace travel_agency_back.DTOs.Requests.Packages
{
    public class PackageRatingDTO
    {
        public int Rating { get; set; } // Rating value, typically between 1 and 5
        public string Comment { get; set; } // Optional comment about the rating
    }
}
