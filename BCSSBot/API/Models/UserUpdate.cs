using Newtonsoft.Json;

namespace BCSSBot.API.Models
{
    public class UserUpdate
    {
        [JsonProperty("userHash")] 
        public long UserHash { get; set; }

        [JsonProperty("discordId")] 
        public long DiscordId { get; set; }

    }
}