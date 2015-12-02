using System;
using System.Collections.Generic;
using System.Linq;
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
        public String Address { get; set; }
        public String ourNickname { get; set; }
        public int Port { get; set; }

        private Paragraph textArea = new Paragraph();
        private Dictionary<String, Action<Socket, String[]>> Commands = new Dictionary<String, Action<Socket, String[]>>();

        private String currentChannel = "";


        public ClientWindow() {
            InitializeComponent();

            MessageBox.Document = new FlowDocument(textArea);

        }
        public void ChangeChatTarget(String tar) {
            Dispatcher.Invoke(new Action(() => {
                ChatTarget.Text = "Chatting with: " + tar;
                currentChannel = tar;
            }));
        }

        public void Init() {
            new Thread(() => {
                connection = new TCPHandler(Address, Port);

                connection.OnConnection += OnConnectionState;
                connection.OnMsg += OnMesseage;

                connection.CreateSocket(false);

                new Thread(() => {
                    while (!IsConnected) {
                        Thread.Sleep(500);
                    }

                    connection.Send("connect " + ourNickname);
                    //connection.Send("join nick #root");
                }).Start();
            }).Start();
        }

        private void ChatBox_KeyPress(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SendChatMessage();
            } else {
                e.Handled = false;
            }

        }

        private void SubmitMSG_Click(object sender, RoutedEventArgs e) {
            SendChatMessage();
        }

        private void NickList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //TODO: allow user info get or messeage
        }

        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //TODO: possibly save text?
            textArea.Inlines.Clear();
            ChangeChatTarget(ChannelList.SelectedValue as String);
        }

        public void OnConnectionState(String ip, ConnectionStatus status) {
            if (status == ConnectionStatus.Succes) {
                Dispatcher.Invoke(new Action(() => {
                    ConnectionOverlay.Visibility = Visibility.Collapsed;
                }));
                IsConnected = true;
            }
        }

        public void OnMesseage(Socket soc, String message) {
            string[] commands = message.Split(new char[] { '\n' });
            foreach (String command in commands) {

                string[] cmdParts = command.Split(new char[] { ' ' }, 2);
                String commandName = cmdParts[0];

                string[] args = null;

                if (cmdParts.Length >= 2) {
                    args = cmdParts[1].Split(new char[] { ' ' });
                }

                if (String.IsNullOrEmpty(commandName) || !Commands.ContainsKey(commandName)) {
                    return;
                }
                Commands[commandName](soc, args);
            }
        }

        public void PrintLine(String text) {
            Dispatcher.Invoke(new Action(() => {
                textArea.Inlines.Add(new Run(text + "\n"));
            }));
        }

        public void SendChatMessage() {
            connection.Send("say " + currentChannel + " " + ourNickname + " " + ChatBox.Text);
            ChatBox.Text = "";
        }

    }
}
