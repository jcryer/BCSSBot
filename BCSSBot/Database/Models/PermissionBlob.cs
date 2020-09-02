using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BCSSBot.Database.DataAccess;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Npgsql;

namespace BCSSBot.API.Models
{
    public class PermissionBlob
    {
        public PermissionBlob()
        {
            Items = new List<PermissionItem>();
        }

        [JsonProperty("items")]
        public List<PermissionItem> Items { get; private set; }
    }

    public class PermissionItem
    {
        public PermissionItem(ulong discordId, PermissionType type)
        {
            DiscordId = discordId;
            Type = type;
        }

        [JsonProperty("discordid")]
        public ulong DiscordId { get; private set; }

        [JsonProperty("type")]
        public PermissionType Type { get; private set; }
    }

    public enum PermissionType
    {
        Role,
        Channel
    }
}