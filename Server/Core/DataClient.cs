using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Protocol;

namespace Server.Core
{
    public class Client
    {
        // // private Guid id = Guid.NewGuid();
        // private ClientInfo info = new(Guid.NewGuid());
        // public Guid Id => info.Id;

        //private TcpListener _listener;
        private TcpClient client;
        // private NetworkStream ns;

        public Client(TcpClient c)
        {
            client = c;
            // ns = client.GetStream();
        }

        // TODO: maybe sent to client his guid when he just connected
        // TODO: add database to save credentials
        // TODO: add some way of maintaining two-way connection between server and client
        //// maybe with two sockets in different Tasks/Threads?
        // public void Process()
        // {
        //     Logger.Info($"Client <{Id}> is being processed.");
        //     try
        //     {
        //         ns = client.GetStream();

        //         var response = ReadMessage();
        //         Logger.Info($"{response.FromUsername}: {response.GetStringData()}");

        //         var msg = $"{response.FromUsername} <{Id}>: {response.GetStringData()}";
        //         WriteMessage(new()
        //         {
        //             // TODO: generate some guid for server
        //             //// maybe save it in server class with this info
        //             //// or create spectial class "Credential" as a format for
        //             //// for keeping user's data and server's data as well.
        //             //// Of course there should be some kind of administration
        //             FromId = Server.Info.Id,
        //             FromUsername = Server.Info.Name,
        //             ToId = Id,
        //             Command = Command.SendPrivateMessage,
        //             Data = Encoding.UTF8.GetBytes(msg)
        //         });
        //         Logger.Info($"response to {response.FromUsername}: \"{msg}\"");
        //     }
        //     catch (Exception ex)
        //     {
        //         //throw;
        //         Logger.Exception(ex.Message);
        //     }
        //     finally
        //     {
        //         Close();
        //         Logger.Info($"Client <{Id}> disconnected.");
        //     }
        // }

        public void Listen()
        {

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
            // if (ns != null)
                // ns.Close();
            if (client != null)
                client.Close();
        }
    }
}