using Newtonsoft.Json;
using Core;
using Server.Data.Models;
using Server.Data.Services;
using Server.TypeExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Core.Contracts;

namespace Server
{
    public class Client
    {
        private DataClient client;
        public bool Active { get; private set; } = false;

        private User _user { get; set; }
        public User User => _user;

        private string _token { get; set; }

        private IUserService userService = new UserService();
        private IActiveUserService activeUserService = new ActiveUserService();
        private ITextMessageService textMessagesService = new TextMessageService();
        private IFileDataService fileDataService = new FileDataService();

        public Client(TcpClient c)
        {
            client = new(c);
            Active = true;
        }

        // So, the algorithm is next:
        // client connects with login and password -> server check credentials,
        //// if exist     => generate token, add it to db and send to client,
        //// if not exist => send message with error
        // all user actions need a token to complete
        // then user send message -> server takes it -> add to db
        // and then clients should request new updates 
        public void Process()
        {
            try
            {
                bool done = false;
                while (!done)
                {
                    var request = client.ReadMessage();
                    switch (request.Command!)
                    {
                        case Command.Login: Login(request);             break;
                        case Command.Logout: done = true;               break;
                        case Command.Register: Register(request);       break;
                        case Command.SendMessage: SendMessage(request); break;
                        case Command.GetMessages: GetMessage(request);  break;
                        case Command.GetMessagesFromTimestamp:
                            GetMessagesFromTimestamp(request); break;
                        case Command.SendFile: SendFile(request);       break;
                        case Command.GetFile: GetFile(request);         break;
                        default: UnknownCommand();                      break;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
                Logger.Exception($"{ex.GetType()}: {ex.Message}");
            }
            finally
            {
                if (_user != null)
                    Logout();
                client.Close();
                Active = false;
                Logger.Info($"Client <{User?.UserId.ToString() ?? "00000000-0000-0000-0000-000000000000"}> disconnected.");
            }
        }

        public void Login(Message message)
        {
            var data = message.GetStringData();

            Logger.Debug("In Login");
            Login login = JsonConvert.DeserializeObject<Login>(data);
            Logger.Debug($"Accepted login data: {login.Name}: {login.Password}");

            _user = userService.GetUserByName(login.Name);
            Command cmd = default;
            string msg = "";

            if (_user == null || _user.Password != login.Password)
            {
                cmd = Command.Error;
                msg = "Incorrect login or password";
            }
            else if (_user.Password == login.Password)
            {
                string token = Guid.NewGuid().ToString();
                Logger.Debug($"New token: {token}");
                activeUserService.AddActiveUser(new()
                {
                    Token = token,
                    UserId = _user.UserId,
                    User = _user
                });

                cmd = Command.Login;
                msg = $"{_user.Name}: {token}";

                _token = token;
            }

            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = cmd,
                Data = msg.ToBytes()
            });
        }

        public void Logout()
        {
            var res = activeUserService.DeleteActiveUserByToken(_token);
            if (res)
                Logger.Debug("Logout: Successfully logged out");
            else
                Logger.Debug("Logout: active user hasn't been deleted");
        }

        public void Register(Message message)
        {
            var data = message.GetStringData();

            Logger.Debug("In Register");
            Login login = JsonConvert.DeserializeObject<Login>(data);

            string msg = "Cannot create user with such name";

            var user = userService.GetUserByName(login.Name);
            if (user == null)
            {
                bool res = userService.AddUser(new User
                {
                    Name = login.Name,
                    Password = login.Password
                });

                if (res)
                    msg = "User successfully created";
            }

            client.WriteMessage(new Message
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
                Data = msg.ToBytes()
            });
        }

        // TODO: change all input and output to Core.Contracts in Data field of message
        public void SendMessage(Message message)
        {
            var (token, text) = SplitTextMessage(message.GetStringData());

            if (!CheckToken(token))
            {
                var msg = "Incorrect token. Please try login again";
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = msg.ToBytes()
                });
            }

            var res = textMessagesService.AddMessage(new()
            {
                Text = text,
                Timestamp = message.Timestamp.Value,
                UserId = _user.UserId,
                User = _user
            });

            if (res)
                Logger.Debug("SendMessage: successfully added new message");
            else
                Logger.Debug("SendMessage: nothing has been added");
        }

        public void GetMessage(Message message)
        {
            var (token, text) = SplitTextMessage(message.GetStringData());

            if (!CheckToken(token))
            {
                var msg = "Incorrect token";
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = msg.ToBytes()
                });
            }

            if (!int.TryParse(text, out int quantity))
                quantity = 100;

            var msgs = textMessagesService
                .GetAllMessages()
                .OrderByDescending(m => m.Timestamp)
                .Take(quantity)
                .Reverse()
                .ToList();

            var r = msgs.Select(m => new Core.Contracts.TextMessage 
            {
                TextMessageId = m.TextMessageId,
                Username = userService.GetUser(m.UserId).Name,
                Text = m.Text,
                Timestamp = m.Timestamp
            }).ToList();

            var response = JsonConvert.SerializeObject(r, Formatting.Indented);

            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
                Data = response.ToBytes()
            });
        }

        public void GetMessagesFromTimestamp(Message message)
        {
            var (token, text) = SplitTextMessage(message.GetStringData());

            if (!CheckToken(token))
            {
                var msg = "Incorrect token";
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = msg.ToBytes()
                });
            }

            if (!DateTime.TryParse(text, out DateTime dt))
                dt = DateTime.Now;

            var msgs = textMessagesService
                .GetAllMessages()
                .Where(m => m.Timestamp > dt)
                .ToList();

            var r = msgs.Select(m => new Core.Contracts.TextMessage
            {
                TextMessageId = m.TextMessageId,
                Username = userService.GetUser(m.UserId).Name,
                Text = m.Text,
                Timestamp = m.Timestamp
            }).ToList();

            var response = JsonConvert.SerializeObject(r, Formatting.Indented);

            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
                Data = response.ToBytes()
            });
        }

        // input:
        //// Data = token:filename:base64_decoded_binary_data
        public void SendFile(Message message)
        {
            var (token, filename, data) = SplitFileMessage(message.GetStringData());

            if (!CheckToken(token))
            {
                var msg = "Incorrect token";
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = msg.ToBytes()
                });
            }

            if (!Directory.Exists("Content"))
                Directory.CreateDirectory("Content");

            File.WriteAllBytes($"Content/{filename}", Convert.FromBase64String(data));
            fileDataService.AddFileData(new() { Path = $"Content/{filename}" });
        }

        // input:
        //// Data = token:requested_file_name
        public void GetFile(Message message)
        {
            var (token, filename) = SplitTextMessage(message.GetStringData());

            if (!CheckToken(token))
            {
                var msg = "Incorrect token";
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = msg.ToBytes()
                });
            }

            var filedata = fileDataService.GetFileData($"Content/{filename}");

            if (filedata is null  || !File.Exists($"Content/{filename}"))
                client.WriteMessage(new()
                {
                    FromId = Server.Info.Id,
                    FromUsername = Server.Info.Name,
                    Command = Command.Error,
                    Data = "No such file".ToBytes()
                });

            var response = new Core.Contracts.FileMessage
            {
                FileMessageId = filedata.FileDataId,
                Filename = filename,
                Data = Convert.ToBase64String(File.ReadAllBytes($"Content/{filename}"))
            };

            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.GetFile,
                Data = JsonConvert.SerializeObject(response).ToBytes()
            });
        }

        public void UnknownCommand()
        {
            string msg = "Incorrect command";
            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
                Data = msg.ToBytes()
            });
        }

        private (string token, string text) SplitTextMessage(string rawMessage)
        {
            var data = rawMessage.Split(new[] { ':' }, 2);
            var token = data[0];
            var text = data[1];

            return (token, text);
        }

        private (string token, string filename, string data) SplitFileMessage(string rawMessage)
        {
            var data = rawMessage.Split(new[] { ':' }, 3);
            var token = data[0];
            var filename = data[1];
            var bin = data[2];

            return (token, filename, bin);
        }

        private bool CheckToken(string token)
        {
            return token == _token;            
        }
    }
}
