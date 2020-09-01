using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCSSBot
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
    }
}