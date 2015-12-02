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

        //TODO: figure out how to gracefully handle all disconects and app exits (kill all process)
        //TODO: stop nick command appearn twice on first join
        //TODO: add private messeging.
        //TODO: add ability to join mutliple rooms and messeage multiple rooms.

        private static Object theLock = new Object();
        TCPHandler connection;
        private bool IsConnected = false;
        public String Address { get; set; }
        public String ourNickname { get; set; }
        public int Port { get; set; }

        private Brush ColChanChange = (Brush)new BrushConverter().ConvertFromString("#4CBBE0");
        private Brush ColChanMsg = (Brush)new BrushConverter().ConvertFromString("#E0714C");
        private Brush ColChanNormal = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");

        private Dictionary<String, Paragraph> channelBuffers = new Dictionary<String, Paragraph>();

        //This is used to interpret commands that come from the server.
        private Dictionary<String, Action<String[]>> ServerCommands = new Dictionary<String, Action<String[]>>();
        //This is used to interpert chat commands and send stuff to the server.
        private Dictionary<String, Action<String[]>> ClientCommands = new Dictionary<String, Action<String[]>>();

        private String currentChannel = "";


        public ClientWindow() {
            InitializeComponent();

            MessageBox.Document = new FlowDocument();

        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            connection.Dispose();
        }


        public void ChangeChatTarget(String tar) {
            Dispatcher.Invoke(new Action(() => {
                ChatTarget.Text = "Chatting with: " + tar;
                //TODO: handle private messeages.
                if (tar.StartsWith("#")) {

                    if (!channelBuffers.ContainsKey(tar))
                        channelBuffers[tar] = new Paragraph();

                    MessageBox.Document.Blocks.Clear();
                    MessageBox.Document.Blocks.Add(channelBuffers[tar]);

                    foreach (Label l in ChannelList.Items) {
                        if (l.Content as String == tar) {
                            ChannelList.SelectedItem = l;
                            break;
                        }
                    }

                    currentChannel = tar;
                    connection.Send("nicklist " + currentChannel);

                    //connection.Send("nicklist " + currentChannel);
                }
            }));
        }

        public void Init() {
            new Thread(() => {
                InitCommands();

                connection = new TCPHandler(Address, Port);

                connection.OnConnection += OnConnectionState;
                connection.OnMsg += OnMesseage;
                connection.OnBadThing += () => {
                    System.Windows.MessageBox.Show("You have been Disconnected from the server.", "Connection Closed", MessageBoxButton.OK);
                    Dispatcher.Invoke(new Action(() => {
                        if (App.Current != null)
                            App.Current.Shutdown();
                    }));
                };

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
            Label selected = (ChannelList.SelectedItem as Label);
            if (selected == null) return;
            selected.Background = ColChanNormal;
            ChangeChatTarget(selected.Content as String);
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

                if (String.IsNullOrEmpty(commandName) || !ServerCommands.ContainsKey(commandName)) {
                    return;
                }
                ServerCommands[commandName](args);
            }
        }

        public void PrintLine(String text, String targetChannel = null) {
            if (String.IsNullOrEmpty(targetChannel))
                targetChannel = currentChannel;

            Dispatcher.Invoke(new Action(() => {
                channelBuffers[targetChannel].Inlines.Add(new Run(text + "\n"));
            }));
        }

        public void SendChatMessage() {
            String text = ChatBox.Text;
            ChatBox.Text = "";

            if (text.StartsWith("/")) {

                //TODO: make this a dictionary.
                string[] cmdParts = text.Substring(1).Split(new char[] { ' ' }, 2);
                String commandName = cmdParts[0];

                string[] args = null;

                if (cmdParts.Length >= 2) {
                    args = cmdParts[1].Split(new char[] { ' ' });
                }

                if (String.IsNullOrEmpty(commandName) || !ClientCommands.ContainsKey(commandName)) {
                    connection.Send("say " + currentChannel + " " + ourNickname + " " + text);
                } else {
                    ClientCommands[commandName](args);
                }
            } else {
                connection.Send("say " + currentChannel + " " + ourNickname + " " + text);
            }
        }

        private void MessageBox_TextChanged(object sender, TextChangedEventArgs e) {
            MessageBox.ScrollToEnd();
        }
    }
}
