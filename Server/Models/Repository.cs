using System;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class Repository : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Identity> Identities { get; set; }

        public Repository() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 
            options.UseSqlite("Data Source=test.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Identity)
                .WithOne(i => i.User)
                .HasForeignKey<Identity>(i => i.UserId);
        }
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public Identity Identity { get; set; }
    }

    public class Identity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Password { get; set; }
    }
}