namespace Messaging
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string Username { get; set; }

        public string EmailSender { get; set; }

        public string SenderPassword { get; set; }
    }
}
