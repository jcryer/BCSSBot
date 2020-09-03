using BCSSBot.API;
using BCSSBot.API.Models;
using BCSSBot.Email;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MimeKit.Encodings;
using Org.BouncyCastle.Math.EC.Rfc7748;
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

        [Command("build"), Description("Creates a set of peer mentor channels"), RequireUserPermissions(Permissions.Administrator)]
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
                var text = await e.Guild.CreateTextChannelAsync("group-" + i, textCategory);
                var voice = await e.Guild.CreateVoiceChannelAsync("Group " + i, voiceCategory);
                await AddPeerGroupToDb("peer-group-" + i, text.Id, voice.Id);
            }

            await e.RespondAsync("Done!");
        }

        public async Task AddPeerGroupToDb(string groupName, ulong textId, ulong voiceId)
        {
            var _db = Settings.GetSettings().BuildContext();

            var perm = new Permission()
            {
                Name = groupName
            };

            var blob = new PermissionBlob();

            blob.Items.Add(new PermissionItem(textId, PermissionType.Channel));
            blob.Items.Add(new PermissionItem(voiceId, PermissionType.Channel));

            perm.SetPermissionBlob(blob);

            await _db.Permissions.AddAsync(perm);
            await _db.SaveChangesAsync();
        }


        [Command("destroy"), Description("Command to delete all peer mentor channels **DANGER**"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Destroy(CommandContext e, string password = "")
        {
            if (password != Settings.GetSettings().DiscordPassword)
            {
                await e.RespondAsync("Requires the admin password.");
                return;
            }

            var db = Settings.GetSettings().BuildContext();

            var permissions = db.Permissions.Where(x => x.Name.StartsWith("peer-group-"));

            bool hasDeletedTextCategory = false;
            bool hasDeletedVoiceCategory = false;

            foreach (var permission in permissions)
            {
                var permissionItems = permission.GetPermissionBlob();

                foreach (var permissionItem in permissionItems.Items.Where(x => x.Type == PermissionType.Channel))
                {
                    var channel = e.Guild.GetChannel(permissionItem.DiscordId);

                    if (!hasDeletedTextCategory && channel.Type == ChannelType.Text)
                    {
                        await channel.Parent.DeleteAsync();
                        hasDeletedTextCategory = true;
                    }

                    if (!hasDeletedVoiceCategory && channel.Type == ChannelType.Voice)
                    {
                        await channel.Parent.DeleteAsync();
                        hasDeletedVoiceCategory = true;
                    }
                   
                    await channel.DeleteAsync();
                }
            }

            db.Memberships.RemoveRange(db.Memberships.Where(x => permissions.Any(y => y.Id == x.Id)));
            db.Permissions.RemoveRange(permissions);
            await db.SaveChangesAsync();
            await e.RespondAsync("Done!");
        }

        [Command("listperms"), Description("Command to list all group IDs"), RequireUserPermissions(Permissions.Administrator)]
        public async Task ListGroups(CommandContext e)
        {
            var _db = Settings.GetSettings().BuildContext();

            var groups = string.Join("\n", _db.Permissions.Select(x => x.Name));
            if (groups.Length == 0)
                groups = "None available";
            await e.RespondAsync($"Available roles: \n```\n{groups}```");
        }

        [Command("addusers")]
        public async Task AddUsers(CommandContext e, [RemainingText]string data)
        {
            var emails = data.Split(' ').ToList();

            var db = Settings.GetSettings().BuildContext();

            var users = emails.Select(x => new User() { Email = x, UserHash = Program.CreateHash(x) });
            await db.Users.AddRangeAsync(users);

            var settings = Settings.GetSettings();
            // send email
            var emailHandler = new EmailSender(settings.EmailUsername, settings.EmailPassword);            

            emailHandler.SendEmails(emails.ToArray(), users.Select(x => x.UserHash.ToString()).ToArray());

            await db.SaveChangesAsync();
            await e.RespondAsync("Done");
        }

        [Command("addperms")]
        public async Task AddPerms(CommandContext e, string groupName, [RemainingText]string data)
        {
            var emailHashes = data.Split(' ').ToList().Select(x =>  Program.CreateHash(x));

            var db = Settings.GetSettings().BuildContext();

            var perm = db.Permissions.FirstOrDefault(x => x.Name == groupName);

            var memberships = emailHashes.Select(x => new Membership() { Id = perm.Id, UserHash = x });

            await db.Memberships.AddRangeAsync(memberships);

            var users = db.Users.Where(x => emailHashes.Contains(x.UserHash) && x.DiscordId != 0 && x.DiscordId != null).ToList();

            users.ForEach(async x => await Bot.ModifyUser(x.DiscordId ?? 0, new Permission[] { perm }, e.Client));

            await db.SaveChangesAsync();
            await e.RespondAsync("Done");
        }

        // next three: test commands
        [Command("createperm"), Description("Command to create a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task CreateGroup(CommandContext e, string groupName)
        {
            await CreateGroup(groupName);

            await e.RespondAsync("Done!");
        }

        [Command("addtoperm"), Description("Command to add to a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddToGroup(CommandContext e, string groupName, DiscordRole role)
        {
            await AddPermissionToGroup(groupName, role.Id, PermissionType.Role);

            await e.RespondAsync("Done!");
        }

        [Command("addtoperm"), Description("Command to add to a permission group"), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddToGroup(CommandContext e, string groupName, DiscordChannel channel)
        {
            await AddPermissionToGroup(groupName, channel.Id, PermissionType.Channel);

            await e.RespondAsync("Done!");
        }

        public async Task CreateGroup(string groupName)
        {
            var _db = Settings.GetSettings().BuildContext();

            var perm = new Permission()
            {
                Name = groupName
            };
            perm.SetPermissionBlob(new PermissionBlob());

            await _db.Permissions.AddAsync(perm);
            await _db.SaveChangesAsync();
        }


        public async Task AddPermissionToGroup(string groupName, ulong discordId, PermissionType type)
        {
            var _db = Settings.GetSettings().BuildContext();
            var perm = _db.Permissions.First(x => x.Name == groupName);

            var blob = perm.GetPermissionBlob();

            blob.Items.Add(new PermissionItem(discordId, type));

            perm.SetPermissionBlob(blob);

            _db.Permissions.Update(perm);
            await _db.SaveChangesAsync();
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
