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
    public class Server
    {
        private TcpListener listener;
        private static AppSettings settings;
        private static int port = 12345;

        public static readonly UserInfo Info = new(new Guid("77777777-7777-7777-7777-777777777777"), "Server");

        private ConcurrentDictionary<Guid, Client> clients = new();

        public Server()
        {
            Init();
        }

        private void Init()
        {
            settings = Configuration.Get();
            port = settings.Port;

            listener = new TcpListener(IPAddress.Any, port);
            Logger.Info($"Server initialized on port {port}");
        }

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
