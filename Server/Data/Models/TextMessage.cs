using System;

namespace Server.Data.Models
{
    public class TextMessage
    {
        public Guid TextMessageId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
