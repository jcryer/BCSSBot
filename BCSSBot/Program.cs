using System;
using System.Threading;
using BCSSBot.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BCSSBot
{
    class Program
    {

        public delegate void WebServerWorker();
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Thread _webServerThread = StartWebServer();
            Console.WriteLine("Test");
        }

        private static Thread StartWebServer()
        {
            WebServerWorker wSWorker = WebServerStarter;
            Thread webServerThread = new Thread(new ThreadStart(wSWorker));
            webServerThread.Start();
            return webServerThread;
        }

        private static void WebServerStarter()
        {
            Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
                .Build()
                .Run();
        } 
    }
}
