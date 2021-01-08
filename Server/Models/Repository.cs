using System;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class Repository : DbContext
    {
        private static readonly Repository _repos = new Repository();

        public DbSet<User> Users { get; set; }
        public DbSet<ActiveUser> ActiveUsers { get; set; }
        //public DbSet<Identity> Identities { get; set; }

        private Repository() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 
            options.UseSqlite("Data Source=test.db");

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(u => u.Identity)
        //        .WithOne(i => i.User)
        //        .HasForeignKey<Identity>(i => i.UserId);
        //}

        public static Repository GetRepository() => _repos;
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        //public Identity Identity { get; set; }
        public ActiveUser ActiveUser { get; set; }
    }

    public class ActiveUser
    {
        public Guid ActiveUserId { get; set; }
        public string Token { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }

    //public class Identity
    //{
    //    public Guid UserId { get; set; }
    //    public User User { get; set; }
    //    public string Password { get; set; }
    //}
}