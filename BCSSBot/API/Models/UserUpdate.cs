using Newtonsoft.Json;

namespace BCSSBot.API.Models
{
    public class UserUpdate
    {
        [JsonProperty("userHash")] 
        public int UserHash { get; set; }

        [JsonProperty("discordId")] 
        public ulong DiscordId { get; set; }

    }
}