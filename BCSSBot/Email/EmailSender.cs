using System;
using BCSSBot.API;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BCSSBot.Email
{
    public class EmailSender : IDisposable
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _username;
        private readonly String _password;
        public EmailSender(string username, string password)
        {
            _smtpClient = new SmtpClient();
            _username = username;
            _password = password;
        }

        public void SendEmail(string recipient, string message, string subject)
        {
            _smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate(_username, _password);
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse("bcssbot@gmail.com"),
                To = {MailboxAddress.Parse(recipient)},
                Subject = subject,
                Body = buildBody(message)
            };
            _smtpClient.Send(email);
            _smtpClient.Disconnect(true);
        }

        public void SendEmails(string[] recipients, string[] links, string[] subject)
        {
            _smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate("bcssbot@gmail.com", "uxkhhtenqumalqyh");
            for (int i = 0; i < recipients.Length; i++)
            {
                var email = new MimeMessage()
                {
                    Sender = MailboxAddress.Parse("bcssbot@gmail.com"),
                    To = {MailboxAddress.Parse(recipients[i])},
                    Subject = subject[i],
                    Body = buildBody(links[i])
                };
                _smtpClient.Send(email);
            }
            _smtpClient.Disconnect(true);
        }
        
        public void Dispose()
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }

        private MimeEntity buildBody(string link)
        {
            var builder = new BodyBuilder();

            builder.TextBody = $@"The BCS server need to link your discord account and email! 
Click the link below to connect them and get your roles!
{link}";

            builder.HtmlBody = Settings.GetSettings().HtmlString;
            return builder.ToMessageBody();
        }
    }
}