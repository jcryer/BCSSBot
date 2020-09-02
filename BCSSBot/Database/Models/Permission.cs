using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
{
    public class Permission
    {
        [Key]
        [Column("discord_permission_id")]
        public ulong DiscordId { get; set; }
        [Column("permission_type")]
        public PermissionType Type { get; set; }
        
        public virtual ICollection<Membership> Memberships { get; }
    }
}