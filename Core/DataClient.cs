using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Core
{
    public class DataClient
    {
        private TcpClient client;
        
        public DataClient(TcpClient c)
        {
            client = c;
        }

        public Message ReadMessage()
        {
            Message msg = null;
            using (BinaryReader r = new BinaryReader(client.GetStream(), Encoding.UTF8, true))
            {
                msg = JsonConvert.DeserializeObject<Message>(r.ReadString());
            }
            return msg;
        }

        public void WriteMessage(Message msg)
        {
            using (BinaryWriter w = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
            {
                w.Write(JsonConvert.SerializeObject(msg, Formatting.Indented));
                w.Flush();
            }
        }

        public void Close()
        {
            if (client != null)
                client.Close();
        }
    }
}