namespace LocalEmailBrowser.MailParsing
{
    using MimeKit;

    public class MessageData
    {
        public string Path { get; set; }

        public MimeMessage Message { get; set; }
    }
}