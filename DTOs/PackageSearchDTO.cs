using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs
{
    public class PackageSearchDTO
    {
        public string? Destination { get; set; }
        public string? Origin { get; set; }
        public DateTime? DepartureDate { get; set; }
    }
}
