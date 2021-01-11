using Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer timer;
        private ObservableCollection<Core.Contracts.TextMessage> History = new();

        private DateTime LastMessage = DateTime.Now.AddHours(-1);

        public MainWindow()
        {
            InitializeComponent();

            MessageBox.Show("To download files send \"/file <filename>\"", "Hint");

            DownloadLastHundredMessages();

            timer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;
            lb_Messages.DataContext = History;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DownloadLatestMessages();
        }

        private void DownloadLastHundredMessages()
        {
            Data.DataClient.WriteMessage(new()
            {
                Command = Command.GetMessages,
                Data = $"{Data.Token}:100".ToBytes()
            });
            LastMessage = DateTime.Now;

            var response = Data.DataClient.ReadMessage();
            var temp = response.GetStringData();
            List<Core.Contracts.TextMessage> msgs = JsonConvert.DeserializeObject<List<Core.Contracts.TextMessage>>(response.GetStringData());

            if (msgs is null)
                return;

            foreach (var msg in msgs)
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    History.Add(msg);
                });
        }

        private void DownloadLatestMessages()
        {
            Data.DataClient.WriteMessage(new()
            {
                Command = Command.GetMessagesFromTimestamp,
                Data = $"{Data.Token}:{LastMessage}".ToBytes()
            });
            LastMessage = DateTime.Now;

            var response = Data.DataClient.ReadMessage();
            var temp = response.GetStringData();
            List<Core.Contracts.TextMessage> msgs = JsonConvert.DeserializeObject<List<Core.Contracts.TextMessage>>(response.GetStringData());

            if (msgs is null)
                return;

            foreach (var msg in msgs)
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    History.Add(msg);
                });
        }

        private void btn_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new();
            if (fd.ShowDialog().Value)
            {
                var index = fd.FileName.LastIndexOf("\\");
                var clearName = fd.FileName[(index + 1)..];

                var file = Convert.ToBase64String(File.ReadAllBytes(fd.FileName));

                Data.DataClient.WriteMessage(new()
                {
                    Command = Command.SendFile,
                    Data = $"{Data.Token}:{clearName}:{file}".ToBytes()
                });

                Data.DataClient.WriteMessage(new()
                {
                    Command = Command.SendMessage,
                    Data = $"{Data.Token}:{Data.Username} uploaded file {clearName}".ToBytes()
                });
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_NewMessage.Text.Trim()))
                return;

            if (tb_NewMessage.Text.Trim().Contains(@"/file"))
            {
                Data.DataClient.WriteMessage(new()
                {
                    Command = Command.GetFile,
                    Data = $"{Data.Token}:{tb_NewMessage.Text.Trim().Split(' ', 2)[1]}".ToBytes()
                });

                var rawdata = Data.DataClient.ReadMessage();
                var stringdata = rawdata.GetStringData();
                var data = JsonConvert.DeserializeObject<Core.Contracts.FileMessage>(stringdata);

                File.WriteAllBytes(data.Filename, Convert.FromBase64String(data.Data));
                MessageBox.Show("File downloaded!", "Success");
            }
            else
            {
                Data.DataClient.WriteMessage(new()
                {
                    Command = Command.SendMessage,
                    Data = $"{Data.Token}:{tb_NewMessage.Text.Trim()}".ToBytes()
                });
            }

            tb_NewMessage.Clear();
        }
    }
}
