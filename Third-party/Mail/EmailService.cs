using System.Net;
using System.Net.Mail;

namespace travel_agency_back.Third_party.Mail
{
    public class EmailService
    {
        public int _smtPort { get; set; } = 587;
        private static string _smtpUser { get; set; } = "noreplyagenciaviagens@gmail.com";
        private static string _smtpPassword = "aron zbty muks wzyu";

        public static void SendPasswordResetEmail(string email, string linkReset)
        {

            //Configura o e-mail e o corpo da mensagem
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpUser);
            mail.To.Add(email);
            mail.Subject = "Solitação de alteração de senha";
            mail.Body = $"";

            //Configura o SMTP client
            SmtpClient smtp = new SmtpClient(host: "smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtp.EnableSsl = true;

            // Envia o e-mail
            smtp.Send(mail);
        }
    }
}
