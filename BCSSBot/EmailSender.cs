using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BCSSBot
{
    public class EmailSender : IDisposable
    {
        private SmtpClient _smtpClient;
        public EmailSender()
        {
            _smtpClient = new SmtpClient();
        }

        public void SendEmail(string recipient, string message, string subject)
        {
            _smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate("bcssbot@gmail.com", "uxkhhtenqumalqyh");
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse("bcssbot@gmail.com"),
                To = {MailboxAddress.Parse(recipient)},
                Subject = subject,
                Body = new TextPart(TextFormat.Plain) {Text = message}
            };
            _smtpClient.Send(email);
            _smtpClient.Disconnect(true);
        }

        public void SendEmails(string[] recipients, string[] messages, string[] subject)
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
                    Body = new TextPart(TextFormat.Plain) {Text = messages[i]}
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
    }
}