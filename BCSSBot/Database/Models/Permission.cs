using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
{
    public class Permission
    {
        [Key]
        [Column("permission_id")]
        public ulong Id { get; set; }

        [Column("permission_name")] 
        public string Name { get; set; }

        [Column("permission_string")] 
        public string JsonBlob { get; set; }
        
        public virtual ICollection<Membership> Memberships { get; }
    }
}