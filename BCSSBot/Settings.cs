using BCSSBot.API.DataAccess;
using Newtonsoft.Json;
using Npgsql;

namespace BCSSBot.API
{
    public class Settings
    {
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
        
        public string BuildConnectionString()
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