using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
{
    public class Membership
    {
        [Key]
        [Column("user_hash")]
        public long userHash { get; set; }
        public User User { get; set; }
        [Key]
        [Column("discord_permission_id")]
        public long discordId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}