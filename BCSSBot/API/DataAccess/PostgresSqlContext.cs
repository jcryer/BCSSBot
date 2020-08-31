using System;
using System.Text.RegularExpressions;
using BCSSBot.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BCSSBot.API.DataAccess
{
    public class PostgresSqlContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Groups { get; set; } 
        
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
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Permission>().ToTable("permissions");
            modelBuilder.Entity<Membership>().ToTable("memberships")
                .HasKey(m => new {m.userHash, m.discordId});

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.userHash);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Permission)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.discordId);
        }
            
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }
}