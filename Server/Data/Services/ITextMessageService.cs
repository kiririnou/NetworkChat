using Server.Data.Models;
using System;
using System.Collections.Generic;

namespace Server.Data.Services
{
    public interface ITextMessageService
    {
        List<TextMessage> GetAllMessages();
        List<TextMessage> GetMessagesFromTimestamp(DateTime timestamp);
        TextMessage GetMessage(Guid id);
        TextMessage GetMessageFromUser(Guid uid);
        bool AddMessage(TextMessage msg);
        bool DeleteMessage(Guid id);
        bool UpdateMessage(TextMessage newMsg);
    }
}
