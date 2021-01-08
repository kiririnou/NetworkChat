using Newtonsoft.Json;
using Protocol;
using Server.Core;
using Server.Models;
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
        private DataClient client;
        // Why do i have those fields?
        private UserInfo info = new(Guid.NewGuid());
        public UserInfo Info => info;

        private User user { get; set; }
        private string _token { get; set; }
        private UserService userService = new UserService();
        private ActiveUserService activeUserService = new ActiveUserService();

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

                var data = request.GetStringData();

                switch (request.Command!)
                {
                    case Command.Login:         Login(data); break;
                    case Command.Register:      Register(data); break;
                    case Command.SendMessage:   break;
                    case Command.GetMessages:   break;
                    case Command.SendFile:      break;
                    case Command.GetFile:       break;
                    default: UnknownCommand();  break;
                }
            }
            catch (Exception ex)
            {
                //throw;
                Logger.Exception(ex.Message);
            }
            finally
            {
                if (user != null)
                    Logout();
                client.Close();
                Logger.Info($"Client <{Info.Id}> disconnected.");
            }
        }

        public void Login(string data)
        {
            Logger.Debug("In Login");
            Login login = JsonConvert.DeserializeObject<Login>(data);
            Logger.Debug($"Accepted login data: {login.Name}: {login.Password}");

            user = userService.GetUserByName(login.Name);
            string msg = "Incorrect login or password";

            if (user == null || user.Password != login.Password)
                msg = "Incorrect login or password";
            else if (user.Password == login.Password)
            {
                string token = Guid.NewGuid().ToString();
                Logger.Debug($"New token: {token}");
                activeUserService.AddActiveUser(new()
                {
                    Token = token,
                    UserId = user.UserId,
                    User = user
                });
                msg = $"Successfully logined: {token}";

                _token = token;
            }

            Logger.Debug(">>> STARTED WRITING DATA");
            client.WriteMessage(new Message
            {
                FromId = Server.Info.Id,
                FromUsername = Server.Info.Name,
                Command = Command.SendMessage,
                Data = msg.ToBytes()
            });
            Logger.Debug(">>> ENDED WRITING DATA");
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
    }
}
