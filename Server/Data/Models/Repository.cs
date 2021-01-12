using Microsoft.EntityFrameworkCore;

namespace Server.Data.Models
{
    public class Repository : DbContext
    {
        private static readonly Repository _repos = new Repository();

        public DbSet<User>        Users         { get; set; }
        public DbSet<ActiveUser>  ActiveUsers   { get; set; }
        public DbSet<TextMessage> TextMessages  { get; set; }
        public DbSet<FileData>    FileDatas     { get; set; }

        private Repository() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 
            options.UseSqlite("Data Source=test.db");

        public static Repository GetRepository() => _repos;
    }
}