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
using BCSSBot.Database.Models;
using DSharpPlus.CommandsNext.Attributes;

namespace BCSSBot.Bots
{
    public class Bot
    {
        private readonly DiscordClient Discord;
        private readonly CommandsNextExtension CommandService;
        private bool Connected;

        public Bot()
        {
            var discordConfig = new DiscordConfiguration
            {
                Token = Settings.GetSettings().DiscordToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { "?" },
                EnableDefaultHelp = false
            };

            Discord = new DiscordClient(discordConfig);

            Discord.GuildAvailable += Discord_GuildAvailable;
            Discord.GuildMemberAdded += Discord_GuildMemberAdded;

            CommandService = Discord.UseCommandsNext(commandsConfig);

            CommandService.RegisterCommands(typeof(Commands));
            CommandService.RegisterCommands(typeof(PeerCommands));
        }

        public async Task<CallbackHolder> RunAsync()
        {
            await Discord.ConnectAsync();

            var holder = new CallbackHolder(async (id, perms) => { await AddUserPermissions(id, perms, Discord); });
            return await Task.FromResult(holder);
        }

        private async Task CommandsService_CommandErrored(CommandErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
            await Task.Delay(0);
        }

        private async Task Discord_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild available: " + e.Guild.Name);

            if (e.Guild.Id == Settings.GetSettings().DiscordServer)
            {
                Connected = true;
            }
            await Task.Delay(0);
        }

        private async Task Discord_GuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs e)
        {
            var db = Settings.GetSettings().BuildContext();

            var user = db.Users.FirstOrDefault(x => x.DiscordId == e.Member.Id);
            var permissions = db.Users.Where(x => x.DiscordId == e.Member.Id)?.SelectMany(x => x.Memberships)?.Select(x => x.Permission)?.ToArray();

            if (user != null)
            {
                await AddUserPermissions(e.Guild, e.Member, permissions);
            }
            await db.SaveChangesAsync();
            await db.DisposeAsync();
        }

        public static async Task<bool> AddUserPermissions(ulong userId, Permission[] permissions, DiscordClient discord)
        {
            try
            {
                DiscordGuild guild = await discord.GetGuildAsync(Settings.GetSettings().DiscordServer);
                DiscordMember member = await guild.GetMemberAsync(userId);

                if (member == null)
                    return false;

                await AddUserPermissions(guild, member, permissions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task AddUserPermissions(DiscordGuild guild, DiscordMember member, Permission[] permissions)
        {
            List<string> roles = new List<string>();
            List<string> channels = new List<string>();
            foreach (var permissionSet in permissions)
            {
                var permissionObj = permissionSet.GetPermissionBlob();
                foreach (var item in permissionObj.Items)
                {
                    if (item.Type == PermissionType.Channel)
                    {
                        var channel = guild.Channels[item.DiscordId];

                        await channel.AddOverwriteAsync(member, Permissions.AccessChannels | Permissions.SendMessages | Permissions.ReadMessageHistory);

                        if (channel.Type == ChannelType.Text)
                            await channel.SendMessageAsync(member.Mention + " has joined the channel!");

                        channels.Add(channel.Name);
                    }
                    else if (item.Type == PermissionType.Role)
                    {
                        var role = guild.Roles[item.DiscordId];
                        await member.GrantRoleAsync(role);
                        roles.Add(role.Name);
                    }
                }
            }

            try
            {
                string message = "";
                if (roles.Count > 0)
                {
                    message += $"You have been given the roles: \n```\n{string.Join("\n", roles)}\n```\n";
                }
                if (channels.Count > 0)
                {
                    message += $"You now have access to the channels: \n```\n{string.Join("\n", channels)}\n```";
                }
                await member.SendMessageAsync(message);
            }
            catch (Exception e)
            {
            }
        }

        public static async Task<bool> RemoveUserPermissions(ulong userId, Permission[] permissions, DiscordClient discord)
        {
            try
            {
                DiscordGuild guild = await discord.GetGuildAsync(Settings.GetSettings().DiscordServer);
                DiscordMember member = await guild.GetMemberAsync(userId);

                if (member == null)
                    return false;

                await RemoveUserPermissions(guild, member, permissions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task RemoveUserPermissions(DiscordGuild guild, DiscordMember member, Permission[] permissions)
        {
            foreach (var permissionSet in permissions)
            {
                var permissionObj = permissionSet.GetPermissionBlob();
                foreach (var item in permissionObj.Items)
                {
                    if (item.Type == PermissionType.Channel)
                    {
                        var overwrites = guild.Channels[item.DiscordId].PermissionOverwrites;
                        foreach (var overwrite in overwrites.Where(x => x.Type == OverwriteType.Member))
                        {
                            if (member == await overwrite.GetMemberAsync() && overwrite.CheckPermission(Permissions.AccessChannels) == PermissionLevel.Allowed)
                            {
                                await overwrite.DeleteAsync();
                                break;
                            }
                        }
                    }
                    else if (item.Type == PermissionType.Role)
                    {
                        await member.RevokeRoleAsync(guild.Roles[item.DiscordId]);
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