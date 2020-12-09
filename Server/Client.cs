using Newtonsoft.Json;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private Guid id = Guid.NewGuid();
        public Guid Id => id;

        //private TcpListener _listener;
        private TcpClient client;
        private NetworkStream ns;

        public Client(TcpClient c)
        {
            client = c;
        }

        public void Process()
        {
            Logger.Info($"Client <{id}> is being processed.");
            try
            {
                ns = client.GetStream();

                //var response = ReadMessage();
                //Logger.Info($"{response.Username}: {response.Msg}");

                //var msg = $"\"{response.Username} <{id}>: {response.Msg}\"";
                //WriteMessage(msg);
                //Logger.Info($"respone to {response.Username}: {msg}");

                var response = ReadProtocolMessage();
                Logger.Info($"{response.FromUsername}: {response.GetStringData()}");

                var msg = $"\"{response.FromUsername} <{id}>: {response.GetStringData()}\"";
                WriteProtocolMessage(new()
                {
                    FromId = new Guid("77777777-7777-7777-7777-777777777777"),
                    FromUsername = "Server",
                    Command = Command.SendPrivateMessage,
                    Data = Encoding.UTF8.GetBytes(msg)
                });
                Logger.Info($"respone to {response.FromUsername}: {msg}");
            }
            catch (Exception ex)
            {
                //Logger.Debug("Catch block");
                //throw;
                Logger.Exception(ex.Message);
            }
            finally
            {
                //Logger.Debug("Finally block");
                Close();
                Logger.Info($"Client <{Id}> closed.");
            }
        }

        private (string Username, string Msg) ReadMessage()
        {
            string username = string.Empty;
            string msg = string.Empty;

            using (BinaryReader r = new(ns, Encoding.UTF8, true))
            {
                username = r.ReadString();
                msg = r.ReadString();
            }
            Logger.Debug($"Readed data: {username} - {msg}");

            return (username, msg);
        }

        private void WriteMessage(string msg)
        {
            using (BinaryWriter w = new(ns, Encoding.UTF8, true))
            {
                w.Write(msg);
                w.Flush();
            }
        }

        private Message ReadProtocolMessage()
        {
            Message msg = null;
            using (BinaryReader r = new(ns, Encoding.UTF8, true))
            {
                msg = JsonConvert.DeserializeObject<Message>(r.ReadString());
            }
            return msg;
        }

        private void WriteProtocolMessage(Message msg)
        {
            using (BinaryWriter w = new(ns, Encoding.UTF8, true))
            {
                w.Write(JsonConvert.SerializeObject(msg));
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
