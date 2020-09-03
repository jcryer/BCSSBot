using Newtonsoft.Json;

namespace BCSSBot.API.Models
{
    public class UserUpdate
    {
        [JsonProperty("userHash")] 
        public string UserHash { get; set; }

        [JsonProperty("discordId")] 
        public string DiscordId { get; set; }

    }
}