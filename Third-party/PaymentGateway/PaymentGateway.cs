
using System.Runtime.CompilerServices;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Third_party.PaymentGateway
{
    public class PaymentGateway
    {
        // Classe para encapsular dados de cartão de forma segura
        public class CardData
        {
            public string CardNumber { get; set; } = string.Empty;
            public string CardHolderName { get; set; } = string.Empty;
            public string ExpiryDate { get; set; } = string.Empty;
            public string CVV { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public int Installments { get; set; } = 1;
        }

        public string CreateNewPayment(PaymentMethod paymentMethod, decimal amount, string email, string? FirstName, string? LastName, string? CPFPassport, string Destino, string Origem, string nomePacote, DateTime InicioViagem, DateTime FimViagem, CardData? cardData = null)
        {
            string status = "";
            string transactionId = "";

            if (paymentMethod == PaymentMethod.CartaoCredito)
            {
                var parcelas = cardData?.Installments ?? 6;

                if (parcelas > 12)
                {
                    amount = amount * 1.05m; // Adiciona 5% de taxa para parcelamento acima de 12 vezes
                }

                status = "Recusado"; // SEMPRE RECUSADO PARA DEMONSTRAÇÃO
                transactionId = $"CC-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                // Envia email do cartão com dados reais
                EmailService.SendCartaoEmail(email, FirstName, LastName, CPFPassport, amount, nomePacote, Destino, Origem, InicioViagem, FimViagem, parcelas, status, transactionId, cardData);
            }

            if (paymentMethod == PaymentMethod.Boleto)
            {
                status = "Pendente"; // SEMPRE PENDENTE PARA DEMONSTRAÇÃO
                transactionId = $"BOL-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                // Envia email do boleto
                EmailService.SendBoletoEmail(email, FirstName, LastName, CPFPassport, amount, nomePacote, Destino, Origem, InicioViagem, FimViagem);
            }

            if (paymentMethod == PaymentMethod.Pix)
            {
                status = "Aprovado"; // SEMPRE APROVADO PARA DEMONSTRAÇÃO
                transactionId = $"PIX-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";

                // Envia email do PIX
                //EmailService.SendPixEmail(email, FirstName, LastName, CPFPassport, amount, nomePacote, Destino, Origem, InicioViagem, FimViagem, transactionId);
            }

            if (paymentMethod == PaymentMethod.CartaoDebito)
            {
                status = "Recusado"; // SEMPRE RECUSADO PARA DEMONSTRAÇÃO  
                transactionId = $"CD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                // Para débito, sempre é 1 parcela
                var debitoCardData = cardData ?? new CardData { Installments = 1 };
                debitoCardData.Installments = 1; // Forçar 1 parcela para débito

                EmailService.SendCartaoEmail(email, FirstName, LastName, CPFPassport, amount, nomePacote, Destino, Origem, InicioViagem, FimViagem, 1, status, transactionId, debitoCardData);
            }

            return $"{status}|{transactionId}"; // Retorna status e ID da transação
        }

        /// <summary>
        /// Método para demonstrar os 4 status FIXOS de pagamento (sem aleatoriedade)
        /// PIX = Sempre Aprovado
        /// Boleto = Sempre Pendente  
        /// Cartão Crédito = Sempre Recusado
        /// Cartão Débito = Sempre Recusado
        /// </summary>
        public void DemonstratePaymentStatuses(string email, string firstName, string lastName, string cpfPassport, string destino, string origem, string nomePacote, DateTime inicioViagem, DateTime fimViagem)
        {
            Console.WriteLine("=== DEMONSTRAÇÃO NEWHORIZON - STATUS FIXOS ===\n");

            // 1. APROVADO - PIX (sempre aprovado)
            var pixResult = CreateNewPayment(PaymentMethod.Pix, 1500.00m, email, firstName, lastName, cpfPassport, destino, origem, nomePacote, inicioViagem, fimViagem);
            Console.WriteLine($"   Resultado: {pixResult}");
            Console.WriteLine("   ✅ Email PIX enviado!\n");

            // 2. PENDENTE - Boleto (sempre pendente)
            var boletoResult = CreateNewPayment(PaymentMethod.Boleto, 2000.00m, email, firstName, lastName, cpfPassport, destino, origem, nomePacote, inicioViagem, fimViagem);
            Console.WriteLine($"   Resultado: {boletoResult}");
            Console.WriteLine("   ⏳ Email BOLETO enviado!\n");

            // 3. RECUSADO - Cartão de Crédito (sempre recusado)
            var creditCardData = new CardData
            {
                CardNumber = "4111111111111111",
                CardHolderName = $"{firstName.ToUpper()} {lastName.ToUpper()}",
                ExpiryDate = "12/28",
                CVV = "123",
                Brand = "Visa",
                Installments = 6
            };
            var cartaoResult = CreateNewPayment(PaymentMethod.CartaoCredito, 3000.00m, email, firstName, lastName, cpfPassport, destino, origem, nomePacote, inicioViagem, fimViagem, creditCardData);
            Console.WriteLine($"   Resultado: {cartaoResult}");
            Console.WriteLine("   ❌ Email CARTÃO CRÉDITO enviado!\n");

            // 4. RECUSADO - Cartão de Débito (sempre recusado)
            var debitCardData = new CardData
            {
                CardNumber = "5555555555554444",
                CardHolderName = $"{firstName.ToUpper()} {lastName.ToUpper()}",
                ExpiryDate = "03/27",
                CVV = "456",
                Brand = "Mastercard",
                Installments = 1
            };
            var debitoResult = CreateNewPayment(PaymentMethod.CartaoDebito, 2500.00m, email, firstName, lastName, cpfPassport, destino, origem, nomePacote, inicioViagem, fimViagem, debitCardData);
            Console.WriteLine($"   Resultado: {debitoResult}");
            Console.WriteLine("   ❌ Email CARTÃO DÉBITO enviado!\n");
        }

        /// <summary>
        /// Método alternativo que permite definir o tipo de pagamento para demonstração específica
        /// </summary>
        public void DemonstrateSpecificPaymentType(PaymentMethod paymentType, string email, string firstName, string lastName, string cpfPassport, string destino, string origem, string nomePacote, DateTime inicioViagem, DateTime fimViagem, CardData? cardData = null)
        {
            string paymentName = paymentType switch
            {
                PaymentMethod.Pix => "PIX",
                PaymentMethod.Boleto => "BOLETO",
                PaymentMethod.CartaoCredito => "CARTÃO CRÉDITO",
                PaymentMethod.CartaoDebito => "CARTÃO DÉBITO",
                _ => "Tipo Desconhecido"
            };

            Console.WriteLine($"=== DEMONSTRAÇÃO: {paymentName.ToUpper()} ===");

            // Usa o valor real passado pelo controller ao invés de valores fixos
            decimal amount = paymentType switch
            {
                PaymentMethod.Pix => 1500.00m,
                PaymentMethod.Boleto => 2000.00m,
                PaymentMethod.CartaoCredito => 3000.00m,
                PaymentMethod.CartaoDebito => 2500.00m,
                _ => 1000.00m
            };

            var result = CreateNewPayment(paymentType, amount, email, firstName, lastName, cpfPassport, destino, origem, nomePacote, inicioViagem, fimViagem, cardData);

            Console.WriteLine($"Resultado: {result}");
            Console.WriteLine("Email enviado com sucesso!");
        }
    }
}
