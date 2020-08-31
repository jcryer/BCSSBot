using Newtonsoft.Json;

namespace BCSSBot.API.Models
{
    public class UserUpdate
    {
        [JsonProperty("userHash")] 
        public long userHash { get; set; }

        [JsonProperty("discordId")] 
        public long discordId { get; set; }

    }
}