using System;
using System.Collections.Generic;

namespace Server.Data.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ActiveUser ActiveUser { get; set; }
        public List<TextMessage> Messages { get; set; }
    }
}
