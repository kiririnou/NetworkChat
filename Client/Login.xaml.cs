using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_Login.Text) || string.IsNullOrEmpty(tb_Password.Text))
            {
                MessageBox.Show("Address, Login or password is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(tb_Address.Text.Trim()))
                Data.Address = "127.0.0.1";
            else
                Data.Address = tb_Address.Text.Trim();

            Data.DataClient.WriteMessage(new()
            {
                Command = Command.Login,
                Data = JsonConvert.SerializeObject(new Core.Contracts.Login
                {
                    Name = tb_Login.Text.Trim(),
                    Password = tb_Password.Text.Trim()
                }, 
                Formatting.Indented).ToBytes()
            });

            var response = Data.DataClient.ReadMessage();

            if (response.Command! == Command.Login)
            {
                var pos = response.GetStringData().LastIndexOf(" ");
                Data.Token = response.GetStringData()[(pos + 1)..];
                Data.Username = response.GetStringData()[..(pos - 1)];

                MessageBox.Show("Successfully logined!", "Success");

                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            Register r = new Register();
            r.Show();
            this.Close();
        }
    }
}
