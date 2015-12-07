using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IRC_Interface {
    /// <summary>
    /// Handled the logic for the server and updates it's UI.
    /// </summary>
    public partial class ServerWindow : Window {
        public int Port { get; set; }
        TCPHandler connection;
        private Dictionary<String, Room> Rooms = new Dictionary<String, Room>();
        private Dictionary<String, Action<Socket, String[]>> Commands = new Dictionary<String, Action<Socket, String[]>>();

        private Paragraph theTextArea = new Paragraph();

        /// <summary>
        /// Initilizes server UI components.
        /// </summary>
        public ServerWindow() {
            InitializeComponent();
            serverLog.Document = new FlowDocument();
            serverLog.Document.Blocks.Clear();
            serverLog.Document.Blocks.Add(theTextArea);
        }

        /// <summary>
        /// This initlizes the listner TCP connection and the #root channel.
        /// </summary>
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

        /// <summary>
        /// When a message is recived, this process it.
        /// This process is identical to the client one, except it sends an error
        /// message to the client if the command isn't found.
        /// </summary>
        /// <param name="soc">The socket that the message was recived on</param>
        /// <param name="message">the message</param>
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

        /// <summary>
        /// Writes a line to the server log.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void PrintLine(String text) {
            try {
                Dispatcher.Invoke(new Action(() => {
                    theTextArea.Inlines.Add(new Run(text + "\n"));
                }));
            } catch (Exception) {
                //For when we're shutting down and try to write.
            }
        }



        /// <summary>
        /// When the kill button is pressed, we gracefully exit the server.
        /// </summary>
        private void killbutton_Click(object sender, RoutedEventArgs e) {
            serverLog.IsEnabled = false;
            killbutton.IsEnabled = false;

            new Thread(() => {
                connection.Dispose();

                //You'll notice these calls throughout the code,
                //when working with WPF, you are not allowed to update the UI directly from anything but 
                //the UI thread. This ensures that the actions we want to take, are taken in the context
                //of the UI thread.
                Dispatcher.Invoke(new Action(() => {
                    App.Current.Shutdown();
                }));
            }).Start();
        }

        /// <summary>
        /// This simply makes sure that the server log always scrolls to the newest message.
        /// </summary>
        private void serverLog_TextChanged(object sender, TextChangedEventArgs e) {
            serverLog.ScrollToEnd();
        }
    }
}
