using System.Collections.Generic;

namespace BCSSBot.API.Models
{
    public class User
    {
        public long UserHash { get; set; }
        public string DiscordId { get; set; }

        public Role[] getRoles;
    }
}