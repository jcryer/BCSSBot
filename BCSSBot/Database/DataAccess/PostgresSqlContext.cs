using BCSSBot.API.Models;
using BCSSBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BCSSBot.Database.DataAccess
{
    public class PostgresSqlContext : DbContext
    {
        // Database tables
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; } 
        public DbSet<Membership> Memberships { get; set; }
        
        private string ConnectionString { get; }
        
        
        public PostgresSqlContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set up tables
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Permission>().ToTable("permissions");
            modelBuilder.Entity<Membership>().ToTable("memberships")
                .HasKey(m => new {userHash = m.UserHash, discordId = m.Id});

            // Set up many to many relationship
            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserHash);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Permission)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.Id);
        }
    }
}