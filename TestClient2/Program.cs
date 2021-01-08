using System;
using Protocol;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;

namespace TestClient2
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Core.Client client = new(new TcpClient("127.0.0.1", 7777));

            Console.WriteLine("Hello World!");

            Console.Write("Login: ");
            string login = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();            

            client.WriteMessage(new()
            {
                Command = Command.Login,
                Data = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new Login 
                    {
                        Name = login,
                        Password = password
                    }, Formatting.Indented))
            });

            //Thread.Sleep(100);

            var response = client.ReadMessage();
            var msg = response.GetStringData();

            Console.WriteLine($"{msg}");
            Console.ReadLine();
        }
    }
}
