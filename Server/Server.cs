using Server.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    // TODO: maybe server should act like API?
    //// then we can create for comfortable interface (some sort of library) for it 
    //// and interact in better way through well-builded requests
    //// should take this idea for better consideration

    // chat
    public class Server
    {
        private TcpListener listener;
        private static AppSettings settings;
        private static int port = 12345;

        public static readonly UserInfo Info = new(new Guid("77777777-7777-7777-7777-777777777777"), "Server");

        // TODO: maybe add some timer, for example 10 minute, to check disconnected client to remove them from repos
        // TODO: change this collection to database
        //// there should be some way lazy data loading
        // TODO: and maybe rewrite it with HTTP protocol and emulate REST API.
        //// It could be a good idea for managing clients via database
        private ConcurrentDictionary<Guid, Client> clients = new();

        public Server()
        {
            Init();
        }

        private void Init()
        {
            settings = Configuration.Get();
            port = settings.Port;

            // TODO: change IPAddress.Any
            //// or not
            listener = new TcpListener(IPAddress.Any, port);
            Logger.Info($"Server initialized on port {port}");
        }

        // TODO: should add some sort of abstraction for private/group chats
        //// For example, create some sorts of rooms with unique id which will be inserted
        //// in Message protocol
        //// Something like "RoomId"
        //// Then we can use ToId to reply on certain messages in private and group chats

        // TODO: add appropriete way to remove clients
        public void Run()
        {
            try
            {
                listener.Start();
                Logger.Info("Waiting for pending connection...");

                while (true)
                {
                    var conn = listener.AcceptTcpClient();
                    Logger.Debug("Server acepted new client");

                    Client client = new(conn);

                    clients.TryAdd(client.Info.Id, client);

                    Task task = new Task(client.Process);
                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
