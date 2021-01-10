using System;
using Protocol;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace TestClient2
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Core.Client client = new(new TcpClient("127.0.0.1", 7777));
            string token = string.Empty;

            Console.WriteLine("Hello World!");

            while (true)
            {
                Console.Write("Enter a command: ");
                var cmd = Console.ReadLine();

                if (cmd == "login")
                {
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
                            },
                            Formatting.Indented))
                    });

                    var response = client.ReadMessage();
                    var msg = response.GetStringData();
                    Console.WriteLine($"{msg}");

                    var pos = msg.LastIndexOf(" ");
                    token = msg.Substring(pos + 1);
                }
                else if (cmd == "logout" || cmd == "exit" || cmd == "q")
                {
                    break;
                    //client.WriteMessage(new()
                    //{
                    //    Command = Command.Logout
                    //});
                    //return;
                }
                else if (cmd == "register")
                {
                    Console.Write("New login: ");
                    string login = Console.ReadLine();
                    Console.Write("New password: ");
                    string password = Console.ReadLine();

                    client.WriteMessage(new()
                    {
                        Command = Command.Register,
                        Data = Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(new Login
                            {
                                Name = login,
                                Password = password
                            },
                            Formatting.Indented))
                    });

                    var response = client.ReadMessage();
                    var msg = response.GetStringData();
                    Console.WriteLine($"{msg}");
                }
                else if (cmd == "msg")
                {
                    Console.Write("Enter msg: ");
                    var msg = Console.ReadLine();

                    client.WriteMessage(new()
                    {
                        Command = Command.SendMessage,
                        Data = Encoding.UTF8.GetBytes($"{token}:{msg}")
                    });
                }
                else if (cmd == "history")
                {
                    client.WriteMessage(new()
                    {
                        Command = Command.GetMessages,
                        Data = Encoding.UTF8.GetBytes($"{token}:10")
                    });

                    var res = client.ReadMessage();
                    Console.WriteLine(res.GetStringData());
                }
                else if (cmd == "sendf")
                {
                    string file = "13 Watashi e.mp3";
                    client.WriteMessage(new()
                    {
                        Command = Command.SendFile,
                        Data = Encoding.UTF8.GetBytes($"{token}:{file}:{Convert.ToBase64String(File.ReadAllBytes(file))}")
                    });
                }
                else if (cmd == "getf")
                {
                    client.WriteMessage(new() 
                    {
                        Command = Command.GetFile,
                        Data = Encoding.UTF8.GetBytes($"{token}:testfile.txt")
                    });

                    var rawdata = client.ReadMessage();

                    var data = rawdata.GetStringData().Split(new[] { ':' }, 2);
                    var bin = data[0];
                    Console.WriteLine("Start writing file");
                    File.WriteAllBytes("downloaded_file.txt", Convert.FromBase64String(bin));

                    Console.WriteLine("Downloaded file");
                }
            }
            client.WriteMessage(new() { Command = Command.Logout });

            Console.ReadLine();
        }
    }
}
