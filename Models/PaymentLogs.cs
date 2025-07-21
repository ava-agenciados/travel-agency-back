namespace travel_agency_back.Models
{
    public class PaymentLogs
    {
        public int Id { get; set; } // Identificador único do log de pagamento
        public int PaymentId { get; set; } // Chave estrangeira para o pagamento associado
        public Payments Payment { get; set; } // Navegação para o pagamento associado
        public string Status { get; set; } = string.Empty; // Status do pagamento (Pendente, Confirmado, Cancelado)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Data e hora da última atualização do log de pagamento
        public string Description { get; set; } = string.Empty; // Descrição do log de pagamento

    }
}
