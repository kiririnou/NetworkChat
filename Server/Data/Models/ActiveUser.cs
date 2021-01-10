using System;

namespace Server.Data.Models
{
    public class ActiveUser
    {
        public Guid ActiveUserId { get; set; }
        public string Token { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
