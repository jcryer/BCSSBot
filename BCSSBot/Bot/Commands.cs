using BCSSBot.API;
using BCSSBot.API.Models;
using BCSSBot.Email;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCSSBot.Bots
{
    class Commands : BaseCommandModule
    {
        [Command("ping"), Description("Test command"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Ping(CommandContext e)
        {
            await e.RespondAsync("Pong");
        }

        [Command("build"), Description("Build command"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Build(CommandContext e, int numGroups)
        {
            if (numGroups <= 0)
            {
                await e.RespondAsync("Must be at least 1 group");
                return;
            }

            if (numGroups > 50)
            {
                await e.RespondAsync("Max 50 groups");
                return;
            }
            
            var textCategory = await e.Guild.CreateChannelCategoryAsync("Peer Mentors", new List<DiscordOverwriteBuilder>() { new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(e.Guild.EveryoneRole) });
            var voiceCategory = await e.Guild.CreateChannelCategoryAsync("Peer Mentors", new List<DiscordOverwriteBuilder>() { new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(e.Guild.EveryoneRole) });

            for (int i = 1; i <= numGroups; i++)
            {
                await e.Guild.CreateTextChannelAsync("group-" + i, textCategory);
                await e.Guild.CreateVoiceChannelAsync("Group " + i, voiceCategory);
            }

            await e.RespondAsync("Done!");
        }

        [Command("destroy"), Description("Destroy command"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Destroy(CommandContext e)
        {
            var channels = await e.Guild.GetChannelsAsync();
            var cat = channels.First(x => x.Name == "Peer Mentors");

            foreach (var x in cat.Children)
            {
                await x.DeleteAsync();
            }
            await cat.DeleteAsync();
            await e.RespondAsync("Done!");
        }

        [Command("creategroup"), Description("Command to create a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task CreateGroup(CommandContext e, string groupName)
        {
            await CreateGroup(groupName);

            await e.RespondAsync("Done!");
        }

        [Command("addtogroup"), Description("Command to add to a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddToGroup(CommandContext e, string groupName, DiscordRole role)
        {
            await AddPermissionToGroup(groupName, role.Id, PermissionType.Role);

            await e.RespondAsync("Done!");
        }

        [Command("addtogroup"), Description("Command to add to a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddToGroup(CommandContext e, string groupName, DiscordChannel channel)
        {
            await AddPermissionToGroup(groupName, channel.Id, PermissionType.Channel);

            await e.RespondAsync("Done!");
        }

        public async Task CreateGroup(string groupName)
        {
            var _db = Settings.GetSettings().CreateContextBuilder().CreateContext();

            var perm = new Permission()
            {
                Name = groupName
            };
            perm.SetPermissionBlob(new PermissionBlob());

            _db.Permissions.Add(perm);
            await _db.SaveChangesAsync();
        }


        public async Task AddPermissionToGroup(string groupName, ulong discordId, PermissionType type)
        {
            var _db = Settings.GetSettings().CreateContextBuilder().CreateContext();
            var perm = _db.Permissions.First(x => x.Name == groupName);

            var blob = perm.GetPermissionBlob();

            blob.Items.Add(new PermissionItem(discordId, type));

            perm.SetPermissionBlob(blob);

            _db.Permissions.Update(perm);
            await _db.SaveChangesAsync();
        }

        [Command("adduser")]
        public async Task AddUser(CommandContext e, string groupName, string email)
        {
            var _db = Settings.GetSettings().CreateContextBuilder().CreateContext();

            var perm = _db.Permissions.First(x => x.Name == groupName);

            _db.Memberships.Add(new Membership()
            {
                Id = perm.Id,
                UserHash = email.GetHashCode()
            });

            _db.Users.Add(new User()
            {
                Email = email,
                UserHash = email.GetHashCode()
            });

            var settings = Settings.GetSettings();
            // send email
            var emailHandler = new EmailSender(settings.EmailUsername, settings.EmailPassword);

            emailHandler.SendEmail(email, "", "Hello: " + (ulong)email.GetHashCode());

            await _db.SaveChangesAsync();
            await e.RespondAsync("Done");
        }

        /*
        [Command("addusers"), Description(""), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddUsers(CommandContext e, string permission, [RemainingText]string users)
        {
            var _db = Settings.GetSettings().CreateContextBuilder().CreateContext();

            _db.Permissions.First(x => x.dis)
            if (users.Length == 0 && e.Message.Attachments.Count > 0)
            {
                string fileName = e.Message.Attachments.First().FileName;
                using WebClient myWebClient = new WebClient();
                myWebClient.DownloadFile(e.Message.Attachments.First().Url, fileName);
                users = File.ReadAllText(fileName);
                File.Delete(fileName);
            }

            var eachLine = users.Split('\n');

            foreach (var user in eachLine)
            {
                // add user to db
            }
        }
        */
    }
}

/*
 
   
     var user = _db.Users.FirstOrDefault(u => u.UserHash == userUpdate.UserHash);

                if (user != null)
                {
                    user.DiscordId = userUpdate.DiscordId;

                    _callbackHolder.Callback(user.DiscordId, _db.Users.Where(x => x.UserHash == userUpdate.UserHash).SelectMany(x => x.Memberships).Select(x => x.Permission).ToArray());

                    _db.Users.Update(user);
                    await _db.SaveChangesAsync();
                    return Ok();
                }
 * */
