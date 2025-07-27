using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travel_agency_back.Models
{
    public class LodgingInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Baths { get; set; }
        public int Beds { get; set; }
        public bool WifiIncluded { get; set; }
        public bool ParkingSpot { get; set; }
        public bool SwimmingPool { get; set; }
        public bool FitnessCenter { get; set; }
        public bool RestaurantOnSite { get; set; }
        public bool PetAllowed { get; set; }
        public bool AirConditioned { get; set; }
        public bool Breakfast { get; set; }
        // Endereço
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string? Complement { get; set; }
        // Relacionamento reverso (opcional)
        public Packages Package { get; set; }
    }
}
