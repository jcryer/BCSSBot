using System;
using System.IO;
using System.Text;
using BCSSBot.Database.DataAccess;
using Newtonsoft.Json;
using Npgsql;

namespace BCSSBot.API
{
    public sealed class Settings
    {
        private static Settings _instance = null;
        public static Settings getSettings()
        {
            if (_instance == null)
            {
                if (!File.Exists("settings.json"))
                {
                    var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                    File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                    Console.WriteLine(
                        "Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                    Console.ReadKey();
                    System.Environment.Exit(1);
                }

                var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
                _instance = JsonConvert.DeserializeObject<Settings>(input);

                // Saving config with same values but updated fields
                var newjson = JsonConvert.SerializeObject(_instance, Formatting.Indented);
                File.WriteAllText("settings.json", newjson, new UTF8Encoding(false));
            }
            return _instance;
        }
        
        [JsonProperty("hostname")]
        public string Hostname { get; private set; }
        
        [JsonProperty("port")]
        public int Port { get; private set; }
        
        [JsonProperty("database")]
        public string Database { get; private set; }
        
        [JsonProperty("username")]
        public string Username { get; private set; }
        
        [JsonProperty("password")]
        public string Password { get; private set; }

        [JsonProperty("discordtoken")]
        public string DiscordToken { get; private set; }

        private string BuildConnectionString()
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = this.Hostname,
                Port = this.Port,
                Database = this.Database,
                Username = this.Username,
                Password = this.Password
            }.ConnectionString;
        }
        
        public DatabaseContextBuilder CreateContextBuilder() => 
            new DatabaseContextBuilder(this.BuildConnectionString());
    }
}