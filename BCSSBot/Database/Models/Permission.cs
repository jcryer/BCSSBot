using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCSSBot.API.Models;
using Newtonsoft.Json;

namespace BCSSBot.Database.Models
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("permission_id")]
        public int Id { get; set; }

        [Column("permission_name")] 
        public string Name { get; set; }

        [Column("permission_string")] 
        public string JsonBlob { get; set; }
        
        public virtual ICollection<Membership> Memberships { get; }

        // Converts JSON blob to Object
        public PermissionBlob GetPermissionBlob()
        {
            return JsonConvert.DeserializeObject<PermissionBlob>(this.JsonBlob);
        }

        // Converts object to JSON blob
        public void SetPermissionBlob(PermissionBlob permissions)
        {
            this.JsonBlob = JsonConvert.SerializeObject(permissions);
        }
    }
}