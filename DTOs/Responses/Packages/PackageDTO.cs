using System;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

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
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime CreatedAt { get; set; }
        public double? DiscountPercent { get; set; } // NOVO: desconto
        public travel_agency_back.DTOs.Packages.LodgingInfoDTO? LodgingInfo { get; set; } // NOVO: info de acomodação
    }
}
