using Newtonsoft.Json;
using Protocol;
using Server.Core;
using Server.Models;
using Server.TypeExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private DataClient client;
        // Why do i have those fields?
        private UserInfo info = new(Guid.NewGuid());
        public UserInfo Info => info;

        public bool Active { get; private set; } = false;

        private User _user { get; set; }
        public User User => _user;

        private string _token { get; set; }

        private IUserService userService = new UserService();
        private IActiveUserService activeUserService = new ActiveUserService();
        private ITextMessageService textMessagesService = new TextMessageService();

        public Client(TcpClient c)
        {
            client = new(c);
            Active = true;
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
                bool done = false;
                while (!done)
                {
                    var request = client.ReadMessage();

                    //var test = JsonConvert.SerializeObject(request, Formatting.Indented);
                    //Logger.Debug(test);
                    //Logger.Debug(request.GetStringData());

                    var data = request.GetStringData();

                    switch (request.Command!)
                    {
                        case Command.Login: Login(data);                break;
                        case Command.Logout: done = true;               break;
                        case Command.Register: Register(data);          break;
                        case Command.SendMessage: SendMessage(request); break;
                        case Command.GetMessages: GetMessage(request);  break;
                        case Command.SendFile: break;
                        case Command.GetFile: break;
                        default: UnknownCommand(); break;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
                Logger.Exception(ex.Message);
            }
            finally
            {
                if (_user != null)
                    Logout();
                client.Close();
                Active = false;
                Logger.Info($"Client <{Info.Id}> disconnected.");
            }
        }

        public void Login(string data)
        {
            Logger.Debug("In Login");
            Login login = JsonConvert.DeserializeObject<Login>(data);
            Logger.Debug($"Accepted login data: {login.Name}: {login.Password}");

            _user = userService.GetUserByName(login.Name);
            string msg = "";

            if (_user == null || _user.Password != login.Password)
            {
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
                msg = $"Successfully logined: {token}";

                _token = token;
            }

            client.WriteMessage(new()
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
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

        public void Register(string data)
        {
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

            var r = msgs.Select(m => new TextMessage 
            {
                TextMessageId = m.TextMessageId,
                Text = m.Text,
                Timestamp = m.Timestamp,
                UserId = m.UserId
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

        //private bool ValidateToken(string msgdata)
        //{
        //    var data = msgdata.Split(new[] { ':' }, 2);
        //    var token = data[0];
        //    var text = data[1];

        //    return CheckToken(token);
        //}

        private bool CheckToken(string token)
        {
            return token == _token;            
        }
    }
}
