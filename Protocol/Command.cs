using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public enum Command
    {
        Login,
        Register,
        SendMessage,
        GetMessages,
        SendFile,
        GetFile,
    }
}
