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
using System.Windows.Shapes;

namespace IRC_Interface {
    /// <summary>
    /// Interaction logic for InitWindow.xaml
    /// </summary>
    public partial class InitWindow : Window {
        public InitWindow() {
            InitializeComponent();

            ServerSelect.IsChecked = true;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            if (ServerSelect.IsChecked != null && ServerSelect.IsChecked == true) {
                ServerWindow win = new ServerWindow() { Port = int.Parse(serverPort.Text) };
                win.Init();
                win.Show();
            } else {
                ClientWindow win = new ClientWindow() { Port = int.Parse(clientPort.Text), Address = clientAddr.Text, ourNickname = clientNick.Text };
                win.Init();
                win.Show();
            }

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }

        private void AppType_Checked(object sender, RoutedEventArgs e) {
            if (sender == ServerSelect) {
                groupboxClient.IsEnabled = false;
                groupboxServer.IsEnabled = true;
            } else {
                groupboxClient.IsEnabled = true;
                groupboxServer.IsEnabled = false;
            }
        }
    }
}
