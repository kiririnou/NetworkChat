using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_Login.Text) || string.IsNullOrEmpty(tb_Password.Text) || string.IsNullOrEmpty(tb_PasswordConfirmation.Text))
            {
                MessageBox.Show("Please enter all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tb_Password.Text.Trim() != tb_PasswordConfirmation.Text.Trim())
            {
                MessageBox.Show("Please check password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Data.DataClient.WriteMessage(new()
            {
                Command = Command.Register,
                Data = JsonConvert.SerializeObject(new Core.Contracts.Login
                {
                    Name = tb_Login.Text.Trim(),
                    Password = tb_Password.Text.Trim()
                },
                Formatting.Indented).ToBytes()
            });

            var response = Data.DataClient.ReadMessage();
            var msg = response.GetStringData();

            MessageBox.Show(msg);

            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
