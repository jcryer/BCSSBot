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
        public int UserHash { get; set; }
        
        // Will be null before user connects
        [Column("discord_id")]
        public ulong? DiscordId { get; set; }

        // Will be null after email has been connected
        [Column("email")] 
        public string Email { get; set; }

        public virtual ICollection<Membership> Memberships { get; set; }
    }
}