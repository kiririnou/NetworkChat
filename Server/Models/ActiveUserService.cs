using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public interface IActiveUserService
    {
        List<ActiveUser> GetAllActiveUsers();
        ActiveUser GetActiveUser(Guid id);
        ActiveUser GetActiveUserByName(string name);
        bool AddActiveUser(ActiveUser user);
        bool DeleteActiveUser(Guid id);
        bool DeleteActiveUserByToken(string token);
        bool UpdateActiveUser(ActiveUser newUser);   
    }

    public class ActiveUserService : IActiveUserService, IDisposable
    {
        private Repository repos; 

        public ActiveUserService()
        {
            repos = Repository.GetRepository();
        }

        public List<ActiveUser> GetAllActiveUsers()
        {
            return repos.ActiveUsers.ToList();
        }

        public ActiveUser GetActiveUser(Guid id)
        {
            return repos.ActiveUsers.SingleOrDefault(u => u.UserId == id);
        }

        public ActiveUser GetActiveUserByName(string name)
        {
            return repos.ActiveUsers.SingleOrDefault(u => u.User.Name == name);
        }

        public bool AddActiveUser(ActiveUser user)
        {
            repos.ActiveUsers.Add(user);
            int created = repos.SaveChanges();
            return created > 0;
        }

        public bool DeleteActiveUser(Guid id)
        {
            var user = GetActiveUser(id);
            if (user == null)
                return false;
            
            repos.ActiveUsers.Remove(user);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool DeleteActiveUserByToken(string token)
        {
            var user = GetAllActiveUsers().Where(u => u.Token.ToUpper() == token.ToUpper()).SingleOrDefault();

            if (user == null)
                return false;

            repos.ActiveUsers.Remove(user);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool DeleteAllActiveUsers()
        {
            var users = GetAllActiveUsers();
            if (users == null || users.Count < 1)
                return false;

            repos.ActiveUsers.RemoveRange(users);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool UpdateActiveUser(ActiveUser newUser)
        {
            repos.ActiveUsers.Update(newUser);
            var updated = repos.SaveChanges();
            return updated > 0;
        }

        public void Dispose()
        {
            repos.Dispose();
        }
    }
}