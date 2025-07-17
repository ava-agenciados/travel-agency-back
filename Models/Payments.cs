namespace travel_agency_back.Models
{
    public class Payments
    {
        public int Id { get; set; } // Identificador único do pagamento
        public int BookingId { get; set; } // Chave estrangeira para a reserva associada
        public Booking Booking { get; set; } // Navegação para a reserva associada
        public decimal Amount { get; set; } // Valor do pagamento
        public string PaymentMethod { get; set; } = string.Empty; // Método de pagamento (Cartão de Crédito, Boleto, etc.)
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow; // Data e hora do pagamento
        public string Status { get; set; } = ""; // Status do pagamento (Pendente, Confirmado, Cancelado)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora de criação do pagamento
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Data e hora da última atualização do pagamento
        public ICollection<PaymentLogs> PaymentLogs { get; set; } = new List<PaymentLogs>(); // Lista de logs de pagamento associados    

    }
}
