using Server.Data.Models;
using System;
using System.Collections.Generic;

namespace Server.Data.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUser(Guid id);
        User GetUserByName(string name);
        bool AddUser(User user);
        bool DeleteUser(Guid id);
        bool UpdateUser(User newUser);
    }
}
