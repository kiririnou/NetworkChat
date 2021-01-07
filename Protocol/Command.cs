using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public enum Command
    {
        SendPrivateMessage,
        //ReceivePrivateMessage,
        SendGroupMessage,
        //ReceiveGroupMessage,
        SendFile,
        //ReceiveFile,
        Login
    }
}
