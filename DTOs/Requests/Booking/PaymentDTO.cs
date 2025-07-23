using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests.Booking
{
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        Boleto,
        Pix
    }
    public class PaymentDTO
    {
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; } // Data do pagamento
        public Guid TransactionId { get; set; } = new Guid();// ID da transação de pagamento
        public string FirstName { get; set; } = string.Empty; // Nome do titular do cartão
        public string LastName { get; set; } = string.Empty; // Sobrenome do titular do cartão  
        public string? CardNumber { get; set; } = string.Empty; // Número do cartão de crédito
        public string? CardHolderName { get; set; } = string.Empty; // Nome do titular do cartão
        public string? ExpirationDate { get; set; } = string.Empty; // Data de validade do cartão (MM/AA)
        public string? CVV { get; set; } = string.Empty; // Código de segurança do cartão (CVV)
        public int Installments { get; set; } = 1; // Número de parcelas para pagamento
        public bool? IsCreditCard { get; set; } = true; // Indica se é um cartão de crédito (true) ou débito (false)
        
        [Required]
        public string CPFPassport { get; set; } = string.Empty; // CPF ou número de passaporte do titular do cartão
        
        [Required]
        [Phone(ErrorMessage = "É necessário informar um número de telefone válido")]
        public string PhoneNumber { get; set; } = string.Empty; // Número de telefone do titular do cartão
    }
}

