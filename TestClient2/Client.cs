using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Core;

namespace Server.Core
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream ns;

        public Client(TcpClient c)
        {
            client = c;
        }

        public Message ReadMessage()
        {
            Message msg = null;
            using (BinaryReader r = new(client.GetStream(), Encoding.UTF8, true))
            {
                msg = JsonConvert.DeserializeObject<Message>(r.ReadString());
            }
            return msg;
        }

        public void WriteMessage(Message msg)
        {
            using (BinaryWriter w = new(client.GetStream(), Encoding.UTF8, true))
            {
                w.Write(JsonConvert.SerializeObject(msg, Formatting.Indented));
                w.Flush();
            }
        }

        public void Close()
        {
            if (ns != null)
                ns.Close();
            if (client != null)
                client.Close();
        }
    }
}