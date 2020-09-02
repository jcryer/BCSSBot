using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BCSSBot.API.Models
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

        public PermissionBlob GetPermissionBlob()
        {
            return JsonConvert.DeserializeObject<PermissionBlob>(this.JsonBlob);
        }

        public void SetPermissionBlob(PermissionBlob permissions)
        {
            this.JsonBlob = JsonConvert.SerializeObject(permissions);
        }
    }
}