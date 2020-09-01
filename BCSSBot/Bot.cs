﻿using BCSSBot.API.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCSSBot.API;
using Org.BouncyCastle.Crypto.Engines;

namespace BCSSBot
{
    public class Bot
    {
        private DiscordClient Discord;
        private CommandsNextExtension CommandsService;
        private ulong MainServer = 301631649978777610;
        private bool Connected;

        public Bot()
        {
            var discordConfig = new DiscordConfiguration
            {
                Token = Settings.getSettings().DiscordToken,
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
            await Task.Delay(0);
        }

        private async Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild available: " + e.Guild.Name);

            if (e.Guild.Id == MainServer)
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
            // if in the db, give relevant roles

            var db = Settings.getSettings().CreateContextBuilder().CreateContext();

            var user = db.Users.FirstOrDefault(x => (ulong)x.DiscordId == e.Member.Id);
            if (user != null) // && hasn't previously been given roles
            {
                await ModifyUser(e.Guild, e.Member, user.Memberships.Select(x => x.Permission).ToArray());
            }
            db.SaveChanges();
        }

        public async Task<bool> ModifyUser(ulong userId, Permission[] permissions)
        {
            try
            {
                DiscordGuild guild = await Discord.GetGuildAsync(MainServer);
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

        public async Task ModifyUser(DiscordGuild guild, DiscordMember member, Permission[] permissions)
        {
            foreach (var role in permissions.Where(x => x.Type == PermissionType.Role))
            {
                await member.GrantRoleAsync(guild.Roles[(ulong)role.DiscordId]);
            }

            foreach (var role in permissions.Where(x => x.Type == PermissionType.Channel))
            {
                await guild.Channels[(ulong)role.DiscordId].AddOverwriteAsync(member, Permissions.AccessChannels | Permissions.SendMessages | Permissions.ReadMessageHistory);
            }
        }

        public bool IsConnected()
        {
            return Connected;
        }
    }
}