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
    }
}
