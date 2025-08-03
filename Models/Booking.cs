namespace travel_agency_back.Models
{
    public class Booking
    {
       public int Id { get; set; } // Identificador único da reserva
        public int UserId { get; set; } // Chave estrangeira para o usuário que fez a reserva
        public User User { get; set; } // Navegação para o usuário associado à reserva
        public int PackageId { get; set; } // Chave estrangeira para o pacote reservado
        public Packages Package { get; set; } // Navegação para o pacote reservado

        public DateTime BookingDate { get; set; } = DateTime.UtcNow; // Data e hora da reserva
        public DateTime TravelDate { get; set; } // Data da viagem
        public DateTime? EndTravel { get; set; } // Data de término da viagem (da reserva)
        public string Status { get; set; } = "Pendente"; // Status da reserva (Pendente, Confirmada, Cancelada)

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Data e hora da última atualização da reserva
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora de criação da reserva
        public ICollection<Companions> Companions { get; set; } = new List<Companions>(); // Lista de acompanhantes associados à reserva
        public ICollection<Payments> Payments { get; set; } = new List<Payments>(); // Lista de pagamentos associados à reserva
        // Opcionais
        public bool HasTravelInsurance { get; set; }
        public bool HasTourGuide { get; set; }
        public bool HasTour { get; set; }
        public bool HasActivities { get; set; }
        public decimal FinalPrice { get; set; } // Valor final calculado
    }
}
