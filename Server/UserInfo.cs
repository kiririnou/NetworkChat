using System;

namespace Server
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public UserInfo() { }

        public UserInfo(Guid id) => Id = id;

        public UserInfo(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}