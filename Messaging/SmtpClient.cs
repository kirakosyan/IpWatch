using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Messaging
{
    public class SMTPClient
    {
        private readonly SmtpSettings settings;

        public SMTPClient(SmtpSettings settings)
        {
            this.settings = settings;
        }

        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            var client = new SmtpClient(this.settings.SmtpServer)
            {
                Port = this.settings.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential
                {
                    UserName = this.settings.Username ?? this.settings.EmailSender,
                    Password = this.settings.SenderPassword
                }
            };

            var m = new MailMessage(this.settings.EmailSender, to, subject, body) { IsBodyHtml = true };

            await client.SendMailAsync(m);
            return true;
        }
    }
}
