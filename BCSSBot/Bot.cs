using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSSBot
{
    internal sealed class Bot
    {
        private DiscordClient Discord;
        private CommandsNextExtension CommandsService;

        public Bot()
        {
            var discordConfig = new DiscordConfiguration
            {
                Token = "",
                TokenType = TokenType.Bot
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { "!" }
            };

            Discord = new DiscordClient(discordConfig);

            Discord.GuildAvailable += Discord_GuildAvailable;
            Discord.SocketErrored += Discord_SocketError;
            Discord.SocketClosed += Discord_SocketClosed;
            Discord.GuildMemberAdded += Discord_GuildMemberAdded;

            this.CommandsService = Discord.UseCommandsNext(commandsConfig);

            CommandsService.RegisterCommands(typeof(Commands));
        }
        public async Task RunAsync()
        {
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild available: " + e.Guild.Name);
            await Task.Delay(0);
        }

        private async Task Discord_SocketError(SocketErrorEventArgs e)
        {
            Console.WriteLine("Socket error");
            await Task.Delay(0);
        }

        private async Task Discord_SocketClosed(SocketCloseEventArgs e)
        {
            Console.WriteLine("Socket closed");
            await Task.Delay(0);
        }


        private async Task Discord_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            // if in the db, give relevant roles

            if (e.Member.Id == 22222)
            {
                await e.Member.GrantRoleAsync(e.Guild.Roles.First(x => x.Value.Name == "BCSS").Value);
            }
        }
    }
}