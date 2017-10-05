namespace LocalEmailBrowser
{
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    using LocalEmailBrowser.MailParsing;

    using MimeKit;

    public partial class Main : Form
    {
        private readonly MailStore mailStore;

        private MessageData[] messages;

        private MessageData selectedMessage;

        public Main()
        {
            this.InitializeComponent();
            this.tbDirectory.Text = @"C:\temp";
            this.mailStore = new MailStore();
            this.PopulateEmailList();
        }

        private void PopulateEmailList()
        {
            this.messages = this.mailStore.GetAll(this.tbDirectory.Text)
                .OrderByDescending(m => m.Message.Date)
                .ToArray();

            var items = this.messages
                .Select(m => m.Message)
                .Select(m => new ListViewItem(new[] {m.Subject, m.Date.ToString("G")}))
                .ToArray();

            this.emailList.Items.Clear();
            this.emailList.Items.AddRange(items);
        }

        private void PopulateMessageDetails()
        {
            var message = this.selectedMessage?.Message;
            this.lblFrom.Text = message?.From.ToString();
            this.lblTo.Text = message?.To.ToString();
            this.lblDate.Text = message?.Date.ToString("F");
            this.lblSubject.Text = message?.Subject;
            this.DisplayHtmlBody(message);
            this.lblAttachments.Text = this.GetAttachmentDetails(message);
        }

        private void DisplayHtmlBody(MimeMessage message)
        {
            this.webBrowser1.Navigate("about:blank");
            if (message != null)
            {
                this.webBrowser1.Document?.OpenNew(false);
                this.webBrowser1.Document?.Write(message.HtmlBody);
            }
            this.webBrowser1.Refresh();
        }

        private string GetAttachmentDetails(MimeMessage message)
        {
            if (message == null)
            {
                return string.Empty;
            }

            if (!message.Attachments.Any())
            {
                return "None";
            }

            var filenames = message.Attachments.OfType<MimePart>().Select(a => a.FileName).ToArray();
            return string.Join("\r\n", filenames);
        }

        private void emailList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var isMessageSelected = this.emailList.SelectedIndices.Count > 0;
            this.btnOpen.Enabled = isMessageSelected;
            this.selectedMessage = isMessageSelected ? this.messages[this.emailList.SelectedIndices[0]] : null;
            this.PopulateMessageDetails();
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            this.PopulateEmailList();
        }

        private void btnOpen_Click(object sender, System.EventArgs e)
        {
            var path = this.selectedMessage.Path;
            Process.Start(path);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme != "about")
            {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }
        }

        private void btnOpenFolder_Click(object sender, System.EventArgs e)
        {
            Process.Start("explorer.exe", this.tbDirectory.Text);
        }
    }
}
