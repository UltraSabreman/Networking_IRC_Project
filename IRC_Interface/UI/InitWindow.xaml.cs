using System.Windows;

namespace IRC_Interface {
    /// <summary>
    /// This window is used to set-up the app, to determine if it's a server or client and
    /// get the appropriate info.
    /// </summary>
    public partial class InitWindow : Window {
        public InitWindow() {
            InitializeComponent();

            ClientSelect.IsChecked = true;
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
