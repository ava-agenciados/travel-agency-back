using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace travel_agency_back.DTOs.Requests.Packages
{
    /// <summary>
    /// DTO utilizado para pesquisar pacotes de viagem.
    /// Permite filtrar pacotes por destino, origem e data de partida.
    /// Todos os campos são opcionais (nullable), o que permite buscas flexíveis.
    /// </summary>
    public class PackageSearchRequestDTO
    {
        /// <summary>
        /// Destino desejado para o pacote de viagem (ex: "Paris", "Fernando de Noronha").
        /// Pode ser nulo se o usuário não quiser filtrar por destino.
        /// </summary>
        public string? Destination { get; set; }

        /// <summary>
        /// Local de origem do viajante (ex: "São Paulo", "Recife").
        /// Pode ser nulo se não for relevante na pesquisa.
        /// </summary>
        public string? Origin { get; set; }

        /// <summary>
        /// Data de partida desejada para o pacote.
        /// É opcional. Se fornecida, pode ser usada para filtrar resultados por data específica.
        /// </summary>
        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime? DepartureDate { get; set; }

        [JsonConverter(typeof(travel_agency_back.Utils.DateTimeJsonConverter))]
        [SwaggerSchema(Format = "date", Description = "Formato: dd/MM/yyyy")]
        public DateTime? ReturnDate { get; set; }
    }
}
