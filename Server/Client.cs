using Newtonsoft.Json;
using Protocol;
using Server.TypeExtensions;
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
        private Core.Client client;
        private UserInfo info = new(Guid.NewGuid());
        public UserInfo Info => info;

        public Client(TcpClient c)
        {
            client = new(c);
        }

        // So, the algorithm is next:
        // client connects with login and password -> server check credentials,
        /// if exist     => generate token, add it to db and send to client,
        /// if not exist => send message with error
        // all user actions need a token to complete
        // then user send message -> server takes it -> add to db
        // and then clients should request new updates 
        public void Process()
        {
            try
            {
                var request = client.ReadMessage();

                var test = JsonConvert.SerializeObject(request, Formatting.Indented);
                Logger.Debug(test);
                Logger.Debug(request.GetStringData());

                switch (request.Command)
                {
                    case Command.Login: Login(); break;
                    default: UnknownCommand(); break;
                }
            }
            catch (Exception ex)
            {
                //throw;
                Logger.Exception(ex.Message);
            }
            finally
            {
                client.Close();
                Logger.Info($"Client <{Info.Id}> disconnected.");
            }
        }

        public void Login()
        {

        }

        public void UnknownCommand()
        {
            string msg = "Incorrect command";
            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                // ToId = Info.Id,
                Command = Command.SendPrivateMessage,
                Data = msg.ToBytes()
            });
        }
    }
}
