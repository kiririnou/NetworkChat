using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public enum Command
    {
        Login,
        Logout,
        Register,
        SendMessage,
        GetMessages,
        GetMessagesFromTimestamp,
        SendFile,
        GetFile,
        Error
    }
}
