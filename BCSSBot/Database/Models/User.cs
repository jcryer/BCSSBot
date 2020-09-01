using System.Collections.Generic;
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
        public ulong DiscordId { get; set; }

        public virtual ICollection<Membership> Memberships { get; set; }
    }
}