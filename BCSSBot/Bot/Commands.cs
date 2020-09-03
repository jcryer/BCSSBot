using BCSSBot.API.Models;
using BCSSBot.Email;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCSSBot.Database.Models;
using System;

namespace BCSSBot.Bots
{
    [Group("peer")]
    [Description("All Peer Mentor Commands")]
    class PeerCommands : BaseCommandModule
    {
        [Command("build"), Description("Creates a set of peer mentor channels and permissions"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Build(CommandContext e, [Description("The number of peer mentor groups to build")]int numGroups)
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

            if (e.Guild.Channels.Any(x => x.Value.Name.Contains("Peer Mentors")))
            {
                await e.RespondAsync("There are already peer mentor channels.");
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
            var db = Settings.GetSettings().BuildContext();

            var perm = new Permission()
            {
                Name = groupName
            };

            var blob = new PermissionBlob();

            blob.Items.Add(new PermissionItem(textId, PermissionType.Channel));
            blob.Items.Add(new PermissionItem(voiceId, PermissionType.Channel));

            perm.SetPermissionBlob(blob);

            await db.Permissions.AddAsync(perm);
            await db.SaveChangesAsync();
            await db.DisposeAsync();

        }

        [Command("destroy"), Description("Deletes all peer mentor channels and permissions - **DANGER**"), RequireUserPermissions(Permissions.Administrator)]
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
            await db.DisposeAsync();

            await e.RespondAsync("Done!");
        }
    }

    class Commands : BaseCommandModule
    {
        [Command("ping"), Description("Test command"), RequireUserPermissions(Permissions.Administrator)]
        public async Task Ping(CommandContext e)
        {
            await e.RespondAsync("Pong!");
        }

        [Command("list"), Description("Lists all permissions"), RequireUserPermissions(Permissions.Administrator)]
        public async Task List(CommandContext e)
        {
            var db = Settings.GetSettings().BuildContext();

            var groups = string.Join("\n", db.Permissions.Select(x => x.Name));
            if (groups.Length == 0)
                groups = "None available";
            await e.RespondAsync($"Available roles: \n```\n{groups}```");
            await db.DisposeAsync();

        }

        [Command("info"), Description("Lists all permission items of a given permission"), RequireUserPermissions(Permissions.Administrator)]
        public async Task ListGroups(CommandContext e, [Description("Name of permission (from list command)")]string permName)
        {
            var db = Settings.GetSettings().BuildContext();

            var perm = db.Permissions.Where(x => x.Name == permName).FirstOrDefault();

            if (perm != null)
            {
                var permItems = perm.GetPermissionBlob().Items;
                var permItemString = string.Join("\n", permItems.Select(x => x.DiscordId + " - " + x.Type));
                if (permItemString.Length == 0)
                    permItemString = "None";
                await e.RespondAsync($"Permission items in {permName}: \n```\n{permItemString}```");
            }
            else
            {
                await e.RespondAsync("No permission by that name found.");
            }
            await db.DisposeAsync();

        }

        [Command("addusers"), Description("Adds a set of emails to the database as new users"), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddUsers(CommandContext e, [RemainingText, Description("List of email addresses separated by spaces")]string emails)
        { 
            var emailList = emails.Split(' ').Select(x => Tuple.Create(x, Program.CreateHash(x)));

            var db = Settings.GetSettings().BuildContext();

            emailList = emailList.Where(x => !db.Users.Any(y => y.UserHash == x.Item2));
            
            var users = emailList.Select(x => new User() { Email = x.Item1, UserHash = x.Item2 });

            await db.Users.AddRangeAsync(users);

            var settings = Settings.GetSettings();
            // send email
            var emailHandler = new EmailSender(settings.EmailUsername, settings.EmailPassword);

            emailHandler.SendEmails(emailList.Select(x => x.Item1).ToArray(), emailList.Select(x => "https://discord.com/oauth2/authorize?client_id=749611213406732370&redirect_uri=https%3A%2F%2Fbcss-su.herokuapp.com&response_type=code&scope=identify&state=" + x.Item2.ToString()).ToArray());

            await db.SaveChangesAsync();
            await db.DisposeAsync();

            await e.RespondAsync("Done");
        }

        [Command("addperm"), Description("Adds a permission to a set of emails in the database."), RequireUserPermissions(Permissions.Administrator)]
        public async Task AddPerms(CommandContext e, [Description("Name of permission (from list command)")]string permName, [RemainingText, Description("List of email addresses separated by spaces")]string emails)
        {
            var emailHashes = emails.Split(' ').Select(x => Program.CreateHash(x));

            var db = Settings.GetSettings().BuildContext();

            var perm = db.Permissions.FirstOrDefault(x => x.Name == permName);

            var memberships = emailHashes.Select(x => new Membership() { Id = perm.Id, UserHash = x });

            await db.Memberships.AddRangeAsync(memberships);

            var users = db.Users.Where(x => emailHashes.Contains(x.UserHash) && x.DiscordId != 0 && x.DiscordId != null).ToList();

            users.ForEach(async x => await Bot.AddUserPermissions(x.DiscordId ?? 0, new Permission[] { perm }, e.Client));

            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await e.RespondAsync("Done");
        }

        [Command("removeperm"), Description("Removes a permission from a set of emails in the database."), RequireUserPermissions(Permissions.Administrator)]
        public async Task RemovePerms(CommandContext e, [Description("Name of permission (from list command)")]string permName, [RemainingText, Description("List of email addresses separated by spaces")]string emails)
        {
            var emailHashes = emails.Split(' ').Select(x => Program.CreateHash(x));

            var db = Settings.GetSettings().BuildContext();

            var perm = db.Permissions.FirstOrDefault(x => x.Name == permName);

            var memberships = db.Memberships.Where(x => x.Id == perm.Id && emailHashes.Contains(x.UserHash));

            db.Memberships.RemoveRange(memberships);

            var users = db.Users.Where(x => emailHashes.Contains(x.UserHash) && x.DiscordId != 0 && x.DiscordId != null).ToList();

            users.ForEach(async x => await Bot.RemoveUserPermissions(x.DiscordId ?? 0, new Permission[] { perm }, e.Client));

            await db.SaveChangesAsync();
            await db.DisposeAsync();

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
            var db = Settings.GetSettings().BuildContext();

            var perm = new Permission()
            {
                Name = groupName
            };
            perm.SetPermissionBlob(new PermissionBlob());

            await db.Permissions.AddAsync(perm);
            await db.SaveChangesAsync();
            await db.DisposeAsync();

        }


        public async Task AddPermissionToGroup(string groupName, ulong discordId, PermissionType type)
        {
            var db = Settings.GetSettings().BuildContext();
            var perm = db.Permissions.First(x => x.Name == groupName);

            var blob = perm.GetPermissionBlob();

            blob.Items.Add(new PermissionItem(discordId, type));

            perm.SetPermissionBlob(blob);

            db.Permissions.Update(perm);
            await db.SaveChangesAsync();
            await db.DisposeAsync();

        }
    }
}