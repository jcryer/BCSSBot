using System;
using BCSSBot.API;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

namespace BCSSBot.Email
{
    // Email sender:
    //    Opens a smtp connection to gmail to send emails with the `/Assets/Email.html` body
    public class EmailSender : IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _username;
        private readonly string _password;

        private const string Subject = "Connect to the BCS server!";
        
        // Username and password need to be Google app passwords    
        //     https://support.google.com/accounts/answer/185833?hl=en
        public EmailSender(string username, string password)
        {
            _smtpClient = new SmtpClient();
            _username = username;
            _password = password;
        }

        // Opens an smtp connection and sends a single email
        public void SendEmail(string recipient, string link)
        {
            _smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate(_username, _password);
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_username),
                To = {MailboxAddress.Parse(recipient)},
                Subject = Subject,
                Body = BuildBody(link)
            };
            _smtpClient.Send(email);
            _smtpClient.Disconnect(true);
        }

        // Opens an smtp connection and sends an array of emails,
        // recipients and links arrays need to be the same length
        public void SendEmails(string[] recipients, string[] links)
        {
            _smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate(_username, _password);
            for (int i = 0; i < recipients.Length; i++)
            {
                var email = new MimeMessage()
                {
                    Sender = MailboxAddress.Parse(_username),
                    To = {MailboxAddress.Parse(recipients[i])},
                    Subject = Subject,
                    Body = BuildBody(links[i])
                };
                _smtpClient.Send(email);
            }
            _smtpClient.Disconnect(true);
        }
        
        // Cleans up the connections to not leave resources, pretty redundetnt
        public void Dispose()
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }

        // Builds the HTML section of the email
        private static MimeEntity BuildBody(string link)
        {
            var builder = new BodyBuilder
            {
                TextBody = $@"The BCS server need to link your discord account and email! 
Click the link below to connect them and get your roles!
{link}"
            };

            var image = builder.LinkedResources.Add("Assets/6a4941269d62fb1ed3a9b8007fa8ed4c.webp");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = Settings.GetSettings().HtmlString.Replace("{EMAILPLACEHOLDER}", link).Replace("{IMAGEHOLDER}", image.ContentId);
            return builder.ToMessageBody();
        }
    }
}