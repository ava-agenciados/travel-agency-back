using System.Runtime.CompilerServices;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Third_party.PaymentGateway
{
    public class PaymentGateway
    {
        public void CreateNewPayment(PaymentMethod paymentMethod, decimal amount, string email, string? FirstName, string? LastName, string? CPFPassport, string Destino, string Origem, string nomePacote, DateTime InicioViagem, DateTime FimViagem , int parcelas = 0)
        {
            if (paymentMethod == PaymentMethod.CartaoCredito)
            {
                if (parcelas > 12)
                {
                    amount = amount * 1.05m; // Adiciona 5% de taxa para parcelamento acima de 12 vezes
                    var StatusParcela = "Recusado"; // Status inicial do pagamento
                    //EmailService.SendPaymentEmail();
                }

                var Status = "Recusado";
                //EmailService.SendPaymentEmail();

            }
            if(paymentMethod == PaymentMethod.Boleto)
            {
                // Simula a criação de um boleto
                var boletoUrl = $"https://boleto.fake/{Guid.NewGuid()}";
                var boletoExpirationDate = DateTime.UtcNow.AddDays(3); // Boleto válido por 3 dias
                var boletoDetails = new
                {
                    Amount = amount,
                    FirstName = FirstName,
                    LastName = LastName,
                    CPFPassport = CPFPassport,
                    ExpirationDate = boletoExpirationDate
                };
                var Status = "Pendente"; // Status inicial do boleto
               EmailService.SendBoletoEmail(email, FirstName, LastName, CPFPassport, amount, nomePacote, Destino, Origem, InicioViagem, FimViagem);
            }
            if(paymentMethod == PaymentMethod.Pix)
            {
                // Simula a criação de um QR Code Pix
                var pixCode = $"pix:{Guid.NewGuid()}";
                var pixExpirationDate = DateTime.UtcNow.AddMinutes(15); // QR Code Pix válido por 15 minutos
                var pixDetails = new
                {
                    Amount = amount,
                    FirstName = FirstName,
                    LastName = LastName,
                    CPFPassport = CPFPassport,
                    ExpirationDate = pixExpirationDate
                };

                var Status = "Aprovado"; // Status inicial do Pix
                //EmailService.SendPaymentEmail();
            }
        }
    }
}
