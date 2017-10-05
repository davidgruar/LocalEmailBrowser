namespace LocalEmailBrowser.MailParsing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using MimeKit;

    public class MailStore
    {
        public IEnumerable<MessageData> GetAll(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return Enumerable.Empty<MessageData>();
            }
            var files = Directory.GetFiles(directory, "*.eml");
            return files.Select(path => new MessageData { Path = path, Message = MimeMessage.Load(path) });
        }
    }
}