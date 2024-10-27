using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace BookRentalService.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public SmtpEmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_smtpUser);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true; 

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                    catch (SmtpException smtpEx)
                    {
                        // Log SMTP exceptions
                        Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Log general exceptions
                        Console.WriteLine($"General Error: {ex.Message}");
                    }
                }
            }
        }
    }
}
