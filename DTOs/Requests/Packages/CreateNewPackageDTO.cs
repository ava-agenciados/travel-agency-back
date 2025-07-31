using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using travel_agency_back.Models;
using travel_agency_back.DTOs.Packages;

namespace travel_agency_back.DTOs.Requests.Packages
{
    public class CreateNewPackageDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Price { get; set; }
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime ActiveFrom { get; set; }
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime ActiveUntil { get; set; }
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime BeginDate { get; set; }
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; }
        public List<PackageMediaDTO> PackageMedia { get; set; } = new();
        public List<PackageRatingDTO> Ratings { get; set; } = new();
        public LodgingInfoDTO LodgingInfo { get; set; } // Nova propriedade para acomodação
        public double? DiscountPercent { get; set; } // Nova propriedade para desconto
    }
}
