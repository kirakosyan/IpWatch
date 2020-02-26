using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
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

        public async Task<bool> SendHostStatusEmail(string to, bool status, string host)
        {
            if(string.IsNullOrWhiteSpace(to))
            {
                return false;
            }

            //using (var fs = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "resources", "ServerStatusTemplate.html")))
            using (var fs = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "resources", "ServerStatusTemplate.html")))
            {
                var template = await fs.ReadToEndAsync();
                var body = template.Replace("{HOST}", host).Replace("{STATUS}", status ? "Is back online" : "Is OFFLINE!");
                var subject = status ? $"Host {host} is back online" : $"Host {host} is offline";
                await SendEmail(to, subject, body);
            }
            return true;
        }

        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            var client = new SmtpClient(this.settings.Server)
            {
                Port = this.settings.Port,
                EnableSsl = true,
                Credentials = new NetworkCredential
                {
                    UserName = this.settings.Username ?? this.settings.EmailSender,
                    Password = this.settings.SenderPassword
                }
            };

            var m = new MailMessage();
            m.From = new MailAddress(this.settings.EmailSender);
            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            foreach (string email in to.Split(new char[] { ' ', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                m.To.Add(email);
            }

            await client.SendMailAsync(m);
            return true;
        }
    }
}
