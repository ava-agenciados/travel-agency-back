namespace travel_agency_back.Models
{
    public class Companions
    {
        public int Id { get; set; } // Identificador único do acompanhante


        public int BookingId { get; set; } // Chave estrangeira para a reserva associada
        public Booking Booking { get; set; } // Navegação para a reserva associada

        public string FullName { get; set; } // Nome do acompanhante
        public DateTime BirthDate { get; set; } // Data de nascimento do acompanhante
        public string DocumentNumber { get; set; } // Número do documento do acompanhante

    }
}
