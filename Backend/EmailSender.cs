using System.Net;
using System.Net.Mail;

namespace Backend
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("onlinelibrarygapt@gmail.com", "jmoz gjkk iwhl idfe")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("onlinelibrarygapt@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Set to true for HTML content
            };
            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}

  
