using System;
using System.Dynamic;

namespace BCSSBot.API.DataAccess
{
    public class DatabaseContextBuilder
    {
        private string ConnectionString { get; }

        public DatabaseContextBuilder(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public PostgresSqlContext CreateContext()
        {
            try
            {
                return new PostgresSqlContext(ConnectionString);
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