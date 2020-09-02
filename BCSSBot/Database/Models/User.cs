using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
{
    public class User
    {
        [Key]
        [Column("user_hash")]
        public ulong UserHash { get; set; }
        [Column("discord_id")]
        public ulong? DiscordId { get; set; }

        [Column("email")] 
        public string Email { get; set; }

        public virtual ICollection<Membership> Memberships { get; set; }
    }
}