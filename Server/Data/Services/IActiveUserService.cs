using Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Services
{
    public interface IActiveUserService
    {
        List<ActiveUser> GetAllActiveUsers();
        ActiveUser GetActiveUser(Guid id);
        ActiveUser GetActiveUserByName(string name);
        bool AddActiveUser(ActiveUser user);
        bool DeleteActiveUser(Guid id);
        bool DeleteActiveUserByToken(string token);
        bool DeleteAllActiveUsers();
        bool UpdateActiveUser(ActiveUser newUser);
    }
}
