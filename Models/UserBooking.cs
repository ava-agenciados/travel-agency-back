namespace travel_agency_back.Models
{

    /// <summary>
    ///pacotes do usuario
    ///esta associada User e Booking
    public class UserBooking
    {
        public int UserId { get; set; } // Identificador do usuário que fez a reserva
        public int BookingId { get; set; } // Identificador do pacote reservado
        public DateTime BookingDate { get; set; } // Data em que a reserva foi feita
        public DateTime DepartureDate { get; set; } // Data de partida do pacote reservado
        public DateTime ReturnDate { get; set; } // Data de retorno do pacote reservado
        public decimal TotalPrice { get; set; } // Preço total da reserva
        // Propriedades de navegação para relacionamentos com outras entidades, se necessário
        /// <summary>
        ///public User User { get; set; } // Referência ao usuário que fez a reserva
        /// </summary>
        //public Packages Package { get; set; } // Referência ao pacote reservado
    }
}
