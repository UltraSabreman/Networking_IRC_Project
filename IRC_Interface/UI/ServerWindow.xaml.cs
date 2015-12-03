using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window {
        public int Port { get; set; }
        TCPHandler connection;
        private Dictionary<String, Room> Rooms = new Dictionary<String, Room>();
        private Dictionary<String, Action<Socket, String[]>> Commands = new Dictionary<String, Action<Socket, String[]>>();
        //private Regex commandSplitter = new Regex("(?:\\\"([^\"]*)\\\"|([^ ]*))(?: ?)", RegexOptions.Compiled);

        private Paragraph theTextArea = new Paragraph();

        public ServerWindow() {
            InitializeComponent();
            serverLog.Document = new FlowDocument();
            serverLog.Document.Blocks.Clear();
            serverLog.Document.Blocks.Add(theTextArea);
        }

        private void killbutton_Click(object sender, RoutedEventArgs e) {
            serverLog.IsEnabled = false;
            killbutton.IsEnabled = false;

            new Thread(() => {
                connection.Dispose();
                Dispatcher.Invoke(new Action(() => {
                    App.Current.Shutdown();
                }));
            }).Start();
        }


        public void Init() {
            InitCommands();

            connection = new TCPHandler();
            connection.Port = Port;
            connection.OnMsg += OnMessage;
            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes)
                    PrintLine(m + ": Connected");
                else
                    PrintLine(m + ": Disconnected");
            };

            connection.CreateSocket(true);

            Room rootRoom = new Room("#root");
            rootRoom.MOTD = "Welcome to the server! All Users are atumatically put into this room. Leaving this room will remove you form the server. Type \"/help\" for more info.";

            Rooms[rootRoom.Name] = rootRoom;
        }

        public void OnMessage(Socket soc, String message) {
            string[] commands = message.Split(new char [] { '\n' });
            foreach (String command in commands) {

                PrintLine((soc.RemoteEndPoint as IPEndPoint).Address + ": " + command);

                string[] cmdParts = command.Split(new char[] { ' ' }, 2);
                String commandName = cmdParts[0];

                string[] args = null;

                if (cmdParts.Length >= 2) {
                    args = cmdParts[1].Split(new char[] { ' ' });
                }

                if (String.IsNullOrEmpty(commandName) || !Commands.ContainsKey(commandName)) {
                    soc.Send(Util.StoB(Errors.BadCommand));
                    return;
                }
                Commands[commandName](soc, args);
            }
        }


        public void PrintLine(String text) {
            Dispatcher.Invoke(new Action(() => {
                theTextArea.Inlines.Add(new Run(text + "\n"));
            }));
        }

        private void serverLog_TextChanged(object sender, TextChangedEventArgs e) {
            serverLog.ScrollToEnd();
        }
    }
}
