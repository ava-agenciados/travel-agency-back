using travel_agency_back.DTOs.Packages;
using travel_agency_back.Models;

namespace travel_agency_back.Models
{


    //lista de pacotes disponiveis NO TOTAL LIVREEEESS
    public class Packages
    {
       
        public int Id { get; set; } // Identificador único do pacote
        public string Name { get; set; } // Nome do pacote
        public string Description { get; set; } // Descrição do pacote
        public decimal Price { get; set; } // Preço do pacote
        public string ImageUrl { get; set; } // URL da imagem do pacote
        
        public DateTime ActiveFrom { get; set; } // Data de início de validade do pacote
        public DateTime ActiveUntil { get; set; } // Data de término de validade do pacote

        public DateTime BeginDate { get; set; } // Data de início do pacote
        public DateTime EndDate { get; set; } // Data de término do pacote

        public string Origin { get; set; } // Local de origem do pacote 
        public string Destination { get; set; } // Local de destino do pacote
        public int Quantity { get; set; } // Quantidade de pacotes disponíveis

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora de criação do pacote
        public bool IsAvailable { get; set; } = true; // Indica se o pacote está disponível para reserva
        public ICollection<Booking> Bookings { get; set; } // Coleção de reservas associadas ao pacote
        public ICollection<Rating> Ratings { get; set; } // Coleção de avaliações associadas ao pacote

        public ICollection<PackageMedia> PackageMedia { get; set; } // Coleção de mídias associadas ao pacote

        // Novas propriedades
        public int? LodgingInfoId { get; set; } // Chave estrangeira para LodgingInfo
        public LodgingInfo? LodgingInfo { get; set; } // Entidade de acomodação
        public double? DiscountPercent { get; set; } // Desconto percentual
    }
}
