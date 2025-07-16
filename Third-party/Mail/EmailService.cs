using System.Net;
using System.Net.Mail;

namespace travel_agency_back.Third_party.Mail
{
    public class EmailService
    {
        public int _smtPort { get; set; } = 587;
        private static string _smtpUser { get; set; } = "noreplyagenciaviagens@gmail.com";
        private static string _smtpPassword = "aron zbty muks wzyu";

        public static void SendPasswordResetEmail(string email, string username, string linkReset)
        {

            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);
            mail.Subject = "Solitação de alteração de senha";
            mail.Body = $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>Solicitação de Alteração de Senha</title>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>AGENCIADOS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>Redefinição de Senha Solicitada</h1>
            <p>Olá {username},</p>
            <p>Recebemos uma solicitação para redefinir a sua senha de acesso. Para continuar com o processo, clique no botão abaixo:</p>
            <div style='text-align:center;margin:25px 0;'>
                <a href='{linkReset}' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px 0;'>Redefinir Minha Senha</a>
            </div>
            <p>Se você não solicitou esta alteração, por favor ignore este e-mail ou entre em contato conosco imediatamente.</p>
            <p>Este link expirará em 24 horas por motivos de segurança.</p>
            <div style='margin-top:15px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@empresa.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2023 Nome da Sua Empresa. Todos os direitos reservados.</p>
            <p>Endereço: Rua Exemplo, 123, Recife - PE</p>
        </div>
    </div>
</body>
</html>
";
            mail.IsBodyHtml = true; // Define que o corpo do e-mail é HTML

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            smtp.Send(mail);
        }





        public static void SendBoletoEmail(string email, string FirstName, string LastName, string CPFPassport, decimal Amount, string NomePacotes, string Destino, string Origem, DateTime InicioViagem, DateTime FimViagem)
        {

            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);
            mail.Subject = "Confirmação de Pagamento de Pacotes";
            mail.Body = $@"
<!DOCTYPE html>
<html lang='pt-br'>
<head>
    <meta charset='UTF-8'>
    <title>Confirmação de Reserva de Pacote</title>
    <script src=""https://cdn.jsdelivr.net/npm/jsbarcode@3.11.0/dist/JsBarcode.all.min.js""></script>
</head>
<body style='background-color:#f5f5f5;padding:20px;'>
    <div style='max-width:600px;margin:0 auto;background-color:#fff;border-radius:8px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.1);'>
        <div style='background-color:#2563eb;padding:20px;text-align:center;color:white;'>
            <div style='font-weight:bold;font-size:24px;'>AGENCIADOS</div>
        </div>
        <div style='padding:30px;color:#333;'>
            <h1 style='color:#2563eb;margin-bottom:20px;font-size:22px;'>Confirmação de Reserva de Pacote</h1>
            <p>Olá {FirstName} {LastName},</p>
            <p>Estamos felizes em confirmar a sua reserva para o pacote:</p>
            <div style='margin:20px 0;'>
                <h2 style='color:#2563eb;'>Pacote: {NomePacotes}</h2>
                <p><strong>Data de Início:</strong> {InicioViagem}</p>
                <p><strong>Data de Término:</strong> {FimViagem}</p>
                <p><strong>Valor Total:</strong> R$ {Amount}</p>
                <p><strong>Método de Pagamento:</strong> Boleto Bancário</p>
                
                <div style='border:1px dashed #ccc;padding:15px;margin:20px 0;font-family:monospace;background:#f9f9f9;'>
                    <h3 style='color:#2563eb;margin-bottom:10px;'>Boleto Bancário (Simulação)</h3>
                    <p><strong>Beneficiário:</strong> Agenciados Turismo</p>
                    <p><strong>CNPJ:</strong> 12.345.678/0001-90</p>
                    <p><strong>Valor:</strong> R$ {Amount}</p>
                    <p><strong>Vencimento:</strong> {DateTime.UtcNow.AddDays(3)}</p>
                    <p><strong>Código de Barras:</strong></p>
                    <div style=""width:100%;"">
                        <svg id=""barcode"" style=""width:100%;height:auto;""></svg>
                    </div>
                    <script>
                        JsBarcode(""#barcode"", ""3419111111111111111111111111111"", {{
                            format: ""CODE128"",
                            lineColor: ""#000"",
                            width: 0.8,
                            height: 30,
                            margin: 0,
                            displayValue: true,
                            font: ""monospace""
                        }});
                    </script>
                    <div style='text-align:center;margin-top:10px;'>
                        <a href='#' style='display:inline-block;background-color:#28a745;color:white !important;text-decoration:none;padding:8px 20px;border-radius:5px;font-weight:bold;'>Imprimir Boleto</a>
                    </div>
                </div>
            </div>
            <p>Para visualizar os detalhes da sua reserva ou pagar via PIX, clique nos botões abaixo:</p>
            <div style='text-align:center;margin:25px 0;display:flex;justify-content:center;gap:15px;'>
                <a href='{{linkDetalhesReserva}}' style='display:inline-block;background-color:#2563eb;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px 0;'>Ver Detalhes</a>
                <a href='{{linkPix}}' style='display:inline-block;background-color:#28a745;color:white !important;text-decoration:none;padding:12px 30px;border-radius:5px;font-weight:bold;margin:10px 0;'>Pagar via PIX</a>
            </div>
            <p style='color:#dc3545;font-weight:bold;margin-bottom:5px;'>Atenção: Seu pacote só será confirmado após o pagamento.</p>
            <p style=""margin-bottom:5px;"">O boleto pode demorar até 3 dias úteis para compensar.</p>
            <p>Se você não reconhece esta reserva, por favor entre em contato conosco imediatamente.</p>
            <div style='margin-top:15px;font-size:14px;'>
                <p>Caso tenha qualquer dúvida, nossa equipe de suporte está disponível para ajudar:</p>
                <p><strong>Email:</strong> suporte@empresa.com</p>
                <p><strong>Telefone:</strong> (11) 1234-5678</p>
            </div>
        </div>
        <div style='background-color:#f0f0f0;padding:15px;text-align:center;font-size:12px;color:#666;'>
            <p>&copy; 2023 Nome da Sua Empresa. Todos os direitos reservados.</p>
            <p>Endereço: Rua Exemplo, 123, Recife - PE</p>
        </div>
    </div>
</body>
</html>

";
            mail.IsBodyHtml = true; // Define que o corpo do e-mail é HTML

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            smtp.Send(mail);
        }
    }
}
