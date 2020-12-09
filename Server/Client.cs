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

                #region deprecated
                //var response = ReadMessage();
                //Logger.Info($"{response.Username}: {response.Msg}");

                //var msg = $"\"{response.Username} <{id}>: {response.Msg}\"";
                //WriteMessage(msg);
                //Logger.Info($"respone to {response.Username}: {msg}");
                #endregion

                var response = ReadMessage();
                Logger.Info($"{response.FromUsername}: {response.GetStringData()}");

                var msg = $"{response.FromUsername} <{id}>: {response.GetStringData()}";
                WriteMessage(new()
                {
                    FromId = new Guid("77777777-7777-7777-7777-777777777777"),
                    FromUsername = "Server",
                    ToId = id,
                    Command = Command.SendPrivateMessage,
                    Data = Encoding.UTF8.GetBytes(msg)
                });
                Logger.Info($"response to {response.FromUsername}: \"{msg}\"");
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
                Logger.Info($"Client <{Id}> disconnected.");
            }
        }

        #region deprecated
        //private (string Username, string Msg) ReadMessage()
        //{
        //    string username = string.Empty;
        //    string msg = string.Empty;

        //    using (BinaryReader r = new(ns, Encoding.UTF8, true))
        //    {
        //        username = r.ReadString();
        //        msg = r.ReadString();
        //    }
        //    Logger.Debug($"Readed data: {username} - {msg}");

        //    return (username, msg);
        //}

        
        //private void WriteMessage(string msg)
        //{
        //    using (BinaryWriter w = new(ns, Encoding.UTF8, true))
        //    {
        //        w.Write(msg);
        //        w.Flush();
        //    }
        //}
        #endregion

        private Message ReadMessage()
        {
            Message msg = null;
            using (BinaryReader r = new(ns, Encoding.UTF8, true))
            {
                msg = JsonConvert.DeserializeObject<Message>(r.ReadString());
            }
            return msg;
        }

        private void WriteMessage(Message msg)
        {
            using (BinaryWriter w = new(ns, Encoding.UTF8, true))
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
