using Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Data.Services
{
    public class UserService : IUserService, IDisposable
    {
        private Repository repos; 

        public UserService()
        {
            repos = Repository.GetRepository();
        }
        
        public List<User> GetAllUsers()
        {
            return repos.Users.ToList();
        }

        public User GetUser(Guid id)
        {
            return repos.Users.SingleOrDefault(u => u.UserId == id);
        }

        public User GetUserByName(string name)
        {
            return repos.Users.SingleOrDefault(u => u.Name == name);
        }

        public bool AddUser(User user)
        {
            repos.Users.Add(user);
            int created = repos.SaveChanges();
            return created > 0;
        }

        public bool DeleteUser(Guid id)
        {
            User user = GetUser(id);
            if (user == null)
                return false;
            
            repos.Users.Remove(user);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool UpdateUser(User newUser)
        {
            repos.Users.Update(newUser);
            var updated = repos.SaveChanges();
            return updated > 0;
        }

        public void Dispose()
        {
            repos.Dispose();
        }
    }
}