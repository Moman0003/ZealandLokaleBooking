using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ZealandLokaleBooking.Services
{
    public static class EmailService
    {
        public static async Task SendEmailAsync(string to, string subject, string body)
        {
            // Indsæt dine SMTP-oplysninger her
            var smtpClient = new SmtpClient("smtp.your-email-provider.com")
            {
                Port = 587, // Typisk port for SMTP med TLS
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com", "Zealand Lokale Booking"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, // Indstil til true, hvis du bruger HTML i emailen
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}