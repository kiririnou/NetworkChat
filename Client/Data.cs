using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Data
    {
        public static string Token { get; set; }

        public static string Username { get; set; }

        public static string Address = new string("");
 
        public static DataClient DataClient = new(new TcpClient(Address, 7777));
    }
}
