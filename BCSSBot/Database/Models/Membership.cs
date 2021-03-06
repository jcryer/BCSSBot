using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCSSBot.API.Models;

namespace BCSSBot.Database.Models
{
    public class Membership
    {
        [Key]
        [Column("user_hash")]
        public int UserHash { get; set; }
        public virtual User User { get; set; }
        [Key]
        [Column("permission_id")]
        public int Id { get; set; }
        public virtual Permission Permission { get; set; }
    }
}