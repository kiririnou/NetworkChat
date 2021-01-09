﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
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
