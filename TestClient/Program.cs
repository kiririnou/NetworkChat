using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
                        w.Write(username);
                        w.Write(msg);
                    }


                    using (BinaryReader r = new(ns, Encoding.UTF8, true))
                    {
                        string response = r.ReadString();
                        Console.WriteLine($"Server: \"{response}\"");
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
