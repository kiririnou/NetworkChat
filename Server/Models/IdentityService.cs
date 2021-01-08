//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Server.Models
//{
//    public interface IIdentityService
//    {
//        List<Identity> GetAllIdentities();
//        Identity GetIdentity(Guid id);
//        bool AddIdentity(Identity identity);
//        bool DeleteIdentity(Guid id);
//        bool UpdateIdentity(Identity newIdentity);  
//    }

//    public class IdentityService : IIdentityService
//    {
//        private Repository repos;

//        public IdentityService()
//        {
//            repos = Repository.GetRepository();
//        }

//        public List<Identity> GetAllIdentities()
//        {
//            return repos.Identities.ToList();
//        }

//        public Identity GetIdentity(Guid id)
//        {
//            return repos.Identities.SingleOrDefault(i => i.UserId == id);
//        }

//        public bool AddIdentity(Identity identity)
//        {
//            repos.Identities.Add(identity);
//            int created = repos.SaveChanges();
//            return created > 0;
//        }

//        public bool DeleteIdentity(Guid id)
//        {
//            var identity = GetIdentity(id);
//            if (identity != null)
//                return false;

//            repos.Identities.Remove(identity);
//            int deleted = repos.SaveChanges();
//            return deleted > 0;
//        }

//        public bool UpdateIdentity(Identity newIdentity)
//        {
//            repos.Identities.Update(newIdentity);
//            int updated = repos.SaveChanges();
//            return updated > 0;
//        }
//    }
//}