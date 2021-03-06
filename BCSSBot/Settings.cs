using System;
using System.IO;
using System.Text;
using BCSSBot.Database.DataAccess;
using Newtonsoft.Json;
using Npgsql;

namespace BCSSBot
{
    public sealed class Settings
    {
        private static Settings _instance;
        public static Settings GetSettings()
        {
            if (_instance != null) return _instance;
            
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                Console.WriteLine(
                    "Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();
                // If settings file doesnt exist. Kill the program.
                Environment.Exit(1);
            }

            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            _instance = JsonConvert.DeserializeObject<Settings>(input);

            _instance.HtmlString = File.ReadAllText("Email/Email.html");
                

            // Saving config with same values but updated fields
            var newjson = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            File.WriteAllText("settings.json", newjson, new UTF8Encoding(false));
            return _instance;
        }
        
        [JsonProperty("hostname")]
        public string Hostname { get; private set; }
        
        [JsonProperty("port")]
        public int Port { get; private set; }
        
        [JsonProperty("database")]
        public string Database { get; private set; }
        
        [JsonProperty("dbusername")]
        public string DbUsername { get; private set; }
        
        [JsonProperty("dbpassword")]
        public string DbPassword { get; private set; }

        [JsonProperty("discordtoken")]
        public string DiscordToken { get; private set; }

        [JsonProperty("discordpassword")]
        public string DiscordPassword { get; private set; }

        [JsonProperty("discordserver")]
        public ulong DiscordServer { get; private set; }

        [JsonProperty("emailusername")]
        public string EmailUsername { get; private set; }

        [JsonProperty("emailpassword")]
        public string EmailPassword { get; private set; }
        
        [JsonProperty("httpaddress")]
        public string HttpAddress { get; private set; }
        
        [JsonIgnore]
        public string HtmlString { get; private set;  }

        private string BuildConnectionString()
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = this.Hostname,
                Port = this.Port,
                Database = this.Database,
                Username = this.DbUsername,
                Password = this.DbPassword
            }.ConnectionString;
        }

        public PostgresSqlContext BuildContext()
        {
            try
            { 
                return new PostgresSqlContext(BuildConnectionString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error setting up database.");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}