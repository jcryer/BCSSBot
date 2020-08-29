using System;
namespace BCSSBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var emailSender = new EmailSender();
            emailSender.SendEmail("arichardsdev@gmail.com", "Test", "Subject");

            const int len = 10;
            string[] strings = new string[len]; 
            string[] recipients = new string[len]; 
            string[] subjects = new string[len]; 
            for (int i = 0; i < len; i++)
            {
                strings[i] = $"{i}";
                subjects[i] = $"subject{i}";
                recipients[i] = "arichardsdev@gmail.com";
            }
            
            emailSender.SendEmails(recipients, strings,subjects);
        }
    }
}