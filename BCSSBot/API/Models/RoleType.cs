using System.ComponentModel;

namespace BCSSBot.API.Models
{
    public enum RoleType
    {
        [Description("Role")]
        Role = 0,
        [Description("Channel")]
        Channel = 1
    }
}