﻿using Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Data.Services
{
    public class TextMessageService : ITextMessageService, IDisposable
    {
        private Repository repos;

        public TextMessageService()
        {
            repos = Repository.GetRepository();
        }

        public List<TextMessage> GetAllMessages()
        {
            return repos.TextMessages.ToList();
        }

        public List<TextMessage> GetMessagesFromTimestamp(DateTime timestamp)
        {
            return repos.TextMessages.Where(m => m.Timestamp >= timestamp).ToList();
        }

        public TextMessage GetMessage(Guid id)
        {
            return repos.TextMessages.SingleOrDefault(m => m.TextMessageId == id);
        }

        public TextMessage GetMessageFromUser(Guid uid)
        {
            return repos.TextMessages.SingleOrDefault(m => m.UserId == uid);
        }

        public bool AddMessage(TextMessage msg)
        {
            repos.TextMessages.Add(msg);
            int created = repos.SaveChanges();
            return created > 0;
        }

        public bool DeleteMessage(Guid id)
        {
            TextMessage msg = GetMessage(id);
            if (msg == null)
                return false;

            repos.TextMessages.Remove(msg);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool UpdateMessage(TextMessage newMsg)
        {
            repos.TextMessages.Update(newMsg);
            var updated = repos.SaveChanges();
            return updated > 0;
        }

        public void Dispose()
        {
            repos.Dispose();
        }
    }
}
