using System.Runtime.InteropServices;
using BCSSBot.API;
using BCSSBot.Email;

namespace BCSSBot
{
    public class TestEmail
    {
        private EmailSender EmailSender =
            new EmailSender(Settings.GetSettings().EmailUsername, Settings.GetSettings().EmailPassword);

        public static void Main(string[] args)
        {
            new TestEmail();
        }

        public TestEmail()
        {
            EmailSender.SendEmails(new string[]{"alfierchrds@gmail.com", "jcryer1234@gmail.com"}, new string[]{"www.google.com", "www.google.com"}, new string[]{"www.google.com", "www.google.com"});
        }
    }
}