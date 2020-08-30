using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BCSSBot
{
    class Commands : BaseCommandModule
    {
        [Command("ping"), Description("Test command")]
        public async Task Ping(CommandContext e)
        {
            await e.RespondAsync("Pong");
        }

        [Command("build"), Description("Build command")]
        public async Task Build(CommandContext e, int numGroups)
        {
            var category = await e.Guild.CreateChannelCategoryAsync("Peer Mentors", new List<DiscordOverwriteBuilder>() { new DiscordOverwriteBuilder().Deny(DSharpPlus.Permissions.AccessChannels).For(e.Guild.EveryoneRole) });

            for (int i = 1; i <= numGroups; i++)
            {
                await e.Guild.CreateTextChannelAsync("group-" + i, category);
            }

            await e.RespondAsync("Pong");
        }

        [Command("destroy"), Description("Destroy command")]
        public async Task Destroy(CommandContext e)
        {
            var channels = await e.Guild.GetChannelsAsync();
            var cat = channels.First(x => x.Name == "Peer Mentors");

            foreach (var x in cat.Children)
            {
                await x.DeleteAsync();
            }
            await cat.DeleteAsync();
            await e.RespondAsync("Pong");
        }
    }
}