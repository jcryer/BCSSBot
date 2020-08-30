using System;
using System.Threading.Tasks;

namespace BCSSBot
{
    class Program
    {
        public static void Main(string[] args) => new Program().Run(args).GetAwaiter().GetResult();

        private async Task Run(string[] args)
        {
            Bot bot = new Bot();
            await bot.RunAsync();
        }
    }
}
