using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BCSSBot.API;
using BCSSBot.API.DataAccess;
using BCSSBot.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;
using System.Threading.Tasks;

namespace BCSSBot
{
    public class Program
    {

        public delegate void WebServerWorker();

        public Settings Settings { get; private set; }
        
        public DatabaseContextBuilder GlobalContextBuilder { get; set; }

        public static void Main(string[] args) => new Program().Run(args).GetAwaiter().GetResult();

        private async Task Run(string[] args)
        {
            /*
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
            */
            
            Program program = new Program();
            program.Start();
            Bot bot = new Bot();
            await bot.RunAsync();
        }

        private async void StartWebServer()
        {
            await BuildWebHost().RunAsync();
        }

        private IHost BuildWebHost()
        {
            var coreContainer = new CoreContainer
            {
                Program = this
            };
            
            var coreContainerService = new ServiceDescriptor(coreContainer.GetType(), coreContainer);
            
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<Startup>()
                        .ConfigureServices(x => x.Add(coreContainerService));
                })
                .Build();
        }

        private void BuildDataBase()
        {
            var db = GlobalContextBuilder.CreateContext();
            /*
            db.Users.Add(new User
            {
                DiscordId = 0,
                Memberships = new List<Membership>(),
                UserHash = 123
            });*/
            db.SaveChanges();
        }
        
        private void Start() 
        {
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();
                return;
            }
            
            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            Settings = JsonConvert.DeserializeObject<Settings>(input);
            
            // Saving config with same values but updated fields
            var newjson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText("settings.json", newjson, new UTF8Encoding(false));
            
            GlobalContextBuilder = Settings.CreateContextBuilder();
            
            BuildDataBase();

            IHost webHost = BuildWebHost();
            webHost.StartAsync().GetAwaiter().GetResult();
        }
    }
}