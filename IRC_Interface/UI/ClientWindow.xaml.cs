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

namespace IRC_Interface {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window {

        private static Object theLock = new Object();
        TCPHandler connection;
        private bool IsConnected = false;

        public ClientWindow() {
            InitializeComponent();

            Console.Clear();
            Util.PrintLine("Mode: ", ConsoleColor.Green, "CLIENT");

            Init();
        }

        private void ChatBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void SubmitMSG_Click(object sender, RoutedEventArgs e) {

        }

        private void NickList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }


        public void OnMesseage(String message) {

        }

        public void Init() {
            Util.Print("Host Address: ");
            String address = Console.ReadLine();
            Util.Print("Host Port: ");
            String port = Console.ReadLine();

            Console.Clear();
            Util.PrintLine("Type '", ConsoleColor.Yellow, "EXIT", "' to exit.");


            if (String.IsNullOrEmpty(address))
                connection = new TCPHandler();
            else if (String.IsNullOrEmpty(port))
                connection = new TCPHandler(address);
            else
                connection = new TCPHandler(address, int.Parse(port));

            Util.PrintLine("YAY 1");

            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes) {
                    IsConnected = true;
                    Util.Print(ConsoleColor.Green, "Connected");
                } else
                    Util.Print(ConsoleColor.Red, "Disconnected");
            };
            Util.PrintLine("YAY 2");

            connection.OnMsg += (s, msg) => {
                //lock (theLock) {
                Util.Print(msg);
                //}
            };
            Util.PrintLine("YAY 3");

            connection.CreateSocket(false);
            Util.PrintLine("YAY 4");

            new Thread(() => {
                Util.PrintLine("YAY 5");

                while (!IsConnected) {
                    Thread.Sleep(500);
                }
                Util.PrintLine("YAY 6");

                connection.Send("connect sabreman");
                Util.PrintLine("YAY 7");

                Thread.Sleep(3000);
                connection.Send("ping");
                Thread.Sleep(3000);


                while (true) {
                    Util.Print("MSG: ");
                    String msg = Console.ReadLine();

                    if (msg == "EXIT") break;
                    connection.Send(msg);

                    Thread.Sleep(1000);
                }
            }).Start();

        }

        public void Draw() {

        }
    }
}
