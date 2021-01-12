using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Core;

namespace TestClient
{
    class Program
    {
        private static readonly int port = 7777;
        private static readonly string addr = "127.0.0.1";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            TcpClient client = null;

            try
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Message: ");
                string msg = Console.ReadLine();

                client = new(addr, port);
                using (NetworkStream ns = client.GetStream())
                {
                    using (BinaryWriter w = new(ns, Encoding.UTF8, true))
                    {
                        Message m = new()
                        {
                            //FromId = new Guid("11111111-1111-1111-1111-111111111111"),
                            FromUsername = username,
                            Command = Command.SendMessage,
                            Data = Encoding.UTF8.GetBytes(msg)
                        };

                        w.Write(JsonConvert.SerializeObject(m));

                        //w.Write(username);
                        //w.Write(msg);
                    }

                    using (BinaryReader r = new(ns, Encoding.UTF8, true))
                    {
                        //string response = r.ReadString();
                        string data = r.ReadString();
                        Message response = JsonConvert.DeserializeObject<Message>(data);

                        Console.WriteLine($"Server: \"{response.GetStringData()}\"");

                        Console.WriteLine($"\n\nFull response:\n{data}");
                    }
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured: {ex.Message}");
            }
            finally
            {
                if (client is not null)
                    client.Close();
            }
            Console.ReadLine();
        }
    }
}
