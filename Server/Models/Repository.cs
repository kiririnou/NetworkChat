using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class Repository : DbContext
    {
        private static readonly Repository _repos = new Repository();

        public DbSet<User> Users { get; set; }
        public DbSet<ActiveUser> ActiveUsers { get; set; }
        public DbSet<TextMessage> TextMessages { get; set; }
        public DbSet<FileData> FileDatas { get; set; }
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
        public List<TextMessage> Messages { get; set; }
    }

    public class ActiveUser
    {
        public Guid ActiveUserId { get; set; }
        public string Token { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }

    public class TextMessage
    {
        public Guid TextMessageId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }

    public class FileData
    {
        public Guid FileDataId { get; set; }
        public string Path { get; set; }
    }

    //public class Identity
    //{
    //    public Guid UserId { get; set; }
    //    public User User { get; set; }
    //    public string Password { get; set; }
    //}
}