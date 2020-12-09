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
            //Logger.Info($"Started processing client with id {id}");
            Logger.Info($"Client <{id}> is being processed.");
            try
            {
                ns = client.GetStream();

                var response = ReadMessage();
                Logger.Info($"{response.Username}: {response.Msg}");

                var msg = $"{response.Username} <{id}>: {response.Msg}";
                WriteMessage(msg);
                Logger.Info($"respone to {response.Username}: {msg}");
            }
            catch (Exception ex)
            {
                Logger.Debug("Catch block");
                //throw;
                Logger.Exception(ex.Message);
            }
            finally
            {
                Logger.Debug("Finally block");
                Close();
                //Logger.Info($"Client with id {Id} has been closed.");
                Logger.Info($"Client <{Id}> closed.");
            }
        }

        private (string Username, string Msg) ReadMessage()
        {
            //byte[] data = Array.Empty<byte>();

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

        public void Close()
        {
            if (ns != null)
                ns.Close();
            if (client != null)
                client.Close();
        }
    }
}
