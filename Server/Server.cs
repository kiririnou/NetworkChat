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
    // chat
    public class Server
    {
        private TcpListener listener;
        private static AppSettings settings;
        private static int port = 12345;

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
            // or not
            listener = new TcpListener(IPAddress.Any, port);
            Logger.Info($"Server initialized on port {port}");
        }

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

                    clients.TryAdd(client.Id, client);

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
                if (listener is not null)
                    listener.Stop();
            }
        }
    }
}
