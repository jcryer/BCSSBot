using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
{
    public class Membership
    {
        [Key]
        [Column("user_hash")]
        public ulong UserHash { get; set; }
        public virtual User User { get; set; }
        [Key]
        [Column("discord_permission_id")]
        public ulong DiscordId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}