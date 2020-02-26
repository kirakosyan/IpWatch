namespace Messaging
{
    public class SmtpSettings
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string EmailSender { get; set; }

        public string SenderPassword { get; set; }
    }
}
