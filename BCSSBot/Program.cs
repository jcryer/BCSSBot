using BCSSBot.API;
using BCSSBot.Bots;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using BCSSBot.Email;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System;
using System.Security.Cryptography;

namespace BCSSBot
{
    public class Program
    {
        private Settings Settings { get; set; }

        private Bot _bot;

        public static async Task Main(string[] args)=> await new Program().Run();

        private async Task Run()
        {
            Settings = Settings.GetSettings();
            
            using var emailSender = new EmailSender(Settings.EmailUsername, Settings.EmailPassword);
            
            _bot = new Bot();
            var callbackHolder = await _bot.RunAsync();
            
            while (!_bot.IsConnected())
            {
                continue;
            }

            using IHost webHost = BuildWebHost(callbackHolder);
            await webHost.RunAsync();
            /*
            Permission[] x = { new Permission() { DiscordId = 552828506036240414, Type = PermissionType.Channel }, new Permission() { DiscordId = 523960418167816196, Type = PermissionType.Channel }, new Permission() { DiscordId = 520715386488881180, Type = PermissionType.Role }, new Permission() { DiscordId = 469269411719675935, Type = PermissionType.Role } };
            Console.WriteLine(await Bot.ModifyUser(126070623855312896, x));
            */
        }

        private IHost BuildWebHost(CallbackHolder holder)
        {
            var service = new ServiceDescriptor(holder.GetType(), holder);
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<Startup>()
                        .ConfigureServices(x => x.Add(service))
                        .UseUrls(Settings.HttpAddress);

                })
                .Build();
        }

        public static int CreateHash(string email)
        {
            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(email));
            return BitConverter.ToInt32(hash, 0);
        }
    }
}