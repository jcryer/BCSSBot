using BCSSBot.API.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCSSBot.API;

namespace BCSSBot.Bots
{
    public class Bot
    {
        private readonly DiscordClient Discord;
        private readonly CommandsNextExtension CommandsService;
        private bool Connected;

        public Bot()
        {
            var discordConfig = new DiscordConfiguration
            {
                Token = Settings.GetSettings().DiscordToken,
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

            CommandsService = Discord.UseCommandsNext(commandsConfig);
            CommandsService.CommandErrored += CommandsService_CommandErrored;

            CommandsService.RegisterCommands(typeof(Commands));
        }

        private async Task CommandsService_CommandErrored(CommandErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
            await Task.Delay(0);
        }

        public async Task<CallbackHolder> RunAsync()
        {
            await Discord.ConnectAsync();

            var holder = new CallbackHolder(async (id, perms) => { await ModifyUser(id, perms, Discord); });
            return await Task.FromResult(holder);
        }

        private async Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild available: " + e.Guild.Name);

            if (e.Guild.Id == Settings.GetSettings().DiscordServer)
            {
                Connected = true;
            }
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
            var db = Settings.GetSettings().CreateContextBuilder().CreateContext();

            var user = db.Users.FirstOrDefault(x => x.DiscordId == e.Member.Id);
            var permissions = db.Users.Where(x => x.DiscordId == e.Member.Id)?.SelectMany(x => x.Memberships)?.Select(x => x.Permission)?.ToArray();

            if (user != null)
            {
                await ModifyUser(e.Guild, e.Member, permissions);
            }
            await db.SaveChangesAsync();
        }

        public static async Task<bool> ModifyUser(ulong userId, Permission[] permissions, DiscordClient discord)
        {
            try
            {
                DiscordGuild guild = await discord.GetGuildAsync(Settings.GetSettings().DiscordServer);
                DiscordMember member = await guild.GetMemberAsync(userId);

                if (member == null)
                    return false;

                await ModifyUser(guild, member, permissions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task ModifyUser(DiscordGuild guild, DiscordMember member, Permission[] permissions)
        {
            foreach (var permissionSet in permissions)
            {
                var permissionObj = permissionSet.GetPermissionBlob();
                foreach (var item in permissionObj.Items)
                {
                    if (item.Type == PermissionType.Channel)
                    {
                        await guild.Channels[item.DiscordId].AddOverwriteAsync(member, Permissions.AccessChannels | Permissions.SendMessages | Permissions.ReadMessageHistory);
                    }
                    else if (item.Type == PermissionType.Role)
                    {
                        await member.GrantRoleAsync(guild.Roles[item.DiscordId]);
                    }
                }
            }
        }

        public bool IsConnected()
        {
            return Connected;
        }
    }
}