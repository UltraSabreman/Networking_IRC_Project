using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace IRC_Interface {
    /// <summary>
    /// The client window. This holds the logic for driving teh client and
    /// updating the client UI.
    /// </summary>
    public partial class ClientWindow : Window {

        TCPHandler connection;

        private bool IsConnected = false;
        public String Address { get; set; }
        public String ourNickname { get; set; }
        public int Port { get; set; }


        //UI colors for misc things.
        private Brush ColChanChange = (Brush)new BrushConverter().ConvertFromString("#4CBBE0");
        private Brush ColChanMsg = (Brush)new BrushConverter().ConvertFromString("#E0714C");
        private Brush ColChanNormal = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");

        //A map of channel buffers to channel names.
        private Dictionary<String, Paragraph> channelBuffers = new Dictionary<String, Paragraph>();

        //This is used to interpret commands that come from the server.
        private Dictionary<String, Action<String[]>> ServerCommands = new Dictionary<String, Action<String[]>>();
        //This is used to interpert chat commands and send stuff to the server.
        private Dictionary<String, Action<String[]>> ClientCommands = new Dictionary<String, Action<String[]>>();

        private String currentchannel = "";

        //Used to ping the server to see if we're still connected.
        System.Timers.Timer ping = new System.Timers.Timer();


        /// <summary>
        /// This initilizes the UI and the pin clock.
        /// </summary>
        public ClientWindow() {
            InitializeComponent();

            MessageBox.Document = new FlowDocument();
            ping.Interval = 1000;
            ping.Elapsed += (o, e) => {
                try {
                    connection.Send("ping");
                } catch (Exception) { }
            };
            ping.Enabled = false;
        }

        public void Init() {
            new Thread(() => {
                InitCommands();

                connection = new TCPHandler(Address, Port);

                connection.OnConnection += OnConnectionState;
                connection.OnMsg += OnMesseage;
                connection.OnBadThing += () => {
                    System.Windows.MessageBox.Show("You have been Disconnected from the server.", "Connection Closed", MessageBoxButton.OK);

                    //You'll notice these calls throughout the code,
                    //when working with WPF, you are not allowed to update the UI directly from anything but 
                    //the UI thread. This ensures that the actions we want to take, are taken in the context
                    //of the UI thread.
                    Dispatcher.Invoke(new Action(() => {
                        Close();
                    }));
                };

                connection.CreateSocket(false);

                new Thread(() => {
                    while (!IsConnected) {
                        Thread.Sleep(500);
                    }

                    connection.Send("connect " + ourNickname);
                    ping.Enabled = true;

                    ping.Start();
                    //connection.Send("join nick #root");
                }).Start();
            }).Start();
        }


        /// <summary>
        /// Change the channel that we are chatting in. 
        /// This invovles chaning the selection in the channel list, removing it's colors
        /// updating the nick list, and loading the appropriate buffer.
        /// </summary>
        /// <param name="tar">The name of the channel we wish to switch to.</param>
        public void ChangeChatTarget(String tar) {
            Dispatcher.Invoke(new Action(() => {
                ChatTarget.Text = "Chatting with: " + tar;

                //Allows to potentually expand and accept private messages.
                if (tar.StartsWith("#")) {
                    //If we dont have a buffer for this channel already, create it.
                    if (!channelBuffers.ContainsKey(tar))
                        channelBuffers[tar] = new Paragraph();

                    //clear out the window and replace it with the needed buffer.
                    MessageBox.Document.Blocks.Clear();
                    MessageBox.Document.Blocks.Add(channelBuffers[tar]);

                    foreach (Label l in channelList.Items) {
                        if (l.Content as String == tar) {
                            channelList.SelectedItem = l;
                            break;
                        }
                    }

                    currentchannel = tar;
                    //Get a list of connected users to this channel.
                    connection.Send("nicklist " + currentchannel);
                }
            }));
        }

        //////////////
        // Events

        /// <summary>
        /// When the window closes we want to gracefull dispose of the connection.
        /// </summary>
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            ping.Enabled = false;
            ping.Stop();

            connection.Dispose();
        }

        /// <summary>
        /// This lets us send our message when hitting enter, instead of having to click "send"
        /// </summary>
        private void ChatBox_KeyPress(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SendChatMessage();
            } else {
                e.Handled = false;
            }
        }


        /// <summary>
        /// This sends the messaged when clicking send
        /// </summary>
        private void SubmitMSG_Click(object sender, RoutedEventArgs e) {
            SendChatMessage();
        }

        /// <summary>
        /// This will allow us to exapnd this app in the future, to include user info and private messages.
        /// </summary>
        private void NickList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //do nothing.
        }

        /// <summary>
        /// This lets us change our current channel when the user clicks on one in the sidebar.
        /// </summary>
        private void channelList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Label selected = (channelList.SelectedItem as Label);
            if (selected == null) return;
            selected.Background = ColChanNormal;
            ChangeChatTarget(selected.Content as String);
        }

        /// <summary>
        /// When we first connect to the server this is used to update the UI.
        /// Could be user for more grecefull error handling.
        /// </summary>
        ///  <param name="ip"> The IP that the connection was established to</param>
        ///  <param name="status"> The status of the connection (connected or not)</param>
        public void OnConnectionState(String ip, ConnectionStatus status) {
            if (status == ConnectionStatus.Succes) {
                Dispatcher.Invoke(new Action(() => {
                    ConnectionOverlay.Visibility = Visibility.Collapsed;
                }));
                IsConnected = true;
            }
        }


        /// <summary>
        /// When the client recives a message/command from the server, it's processed here.
        /// </summary>
        /// <param name="soc">The socket that the message was recived on. This allows for private messages.</param>
        /// <param name="message">The message itself.</param>
        public void OnMesseage(Socket soc, String message) {
            //First we split the message by newlines, incase multiple messages where queued together.
            string[] commands = message.Split(new char[] { '\n' });
            foreach (String command in commands) {
                //Then we split the current message by spaces.
                string[] cmdParts = command.Split(new char[] { ' ' }, 2);
                String commandName = cmdParts[0]; //The first word is our command

                //If we have more then one part to our message, we make a list of argument strings.
                string[] args = null;
                if (cmdParts.Length > 1) {
                    args = cmdParts[1].Split(new char[] { ' ' });
                }

                //Then we check if the command actually exists in our command->function map, if not, we do nothing.
                if (String.IsNullOrEmpty(commandName) || !ServerCommands.ContainsKey(commandName)) {
                    return;
                }
                //otherwise we execute it.
                ServerCommands[commandName](args);
            }
        }

        /// <summary>
        /// This lets us print a line of text to our buffer.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="targetchannel"></param>
        public void PrintLine(String text, String targetchannel = null) {
            if (String.IsNullOrEmpty(targetchannel))
                targetchannel = currentchannel;

            Dispatcher.Invoke(new Action(() => {
                channelBuffers[targetchannel].Inlines.Add(new Run(text + "\n"));
            }));
        }

        /// <summary>
        /// This sends the contetns of the chat box as a "say target channel msg" command
        /// OR interprets it as a client command if it's prefixed with a "/"
        /// </summary>
        public void SendChatMessage() {
            String text = ChatBox.Text;
            ChatBox.Text = "";

            if (text.StartsWith("/")) {
                //Same as above, we split the command by spaces.
                string[] cmdParts = text.Substring(1).Split(new char[] { ' ' }, 2);
                String commandName = cmdParts[0];

                //Check if we need to make an argument list
                string[] args = null;
                if (cmdParts.Length > 1) {
                    args = cmdParts[1].Split(new char[] { ' ' });
                }

                //And run it if its in our map, otherwise we send it as a chat message.
                if (String.IsNullOrEmpty(commandName) || !ClientCommands.ContainsKey(commandName)) {
                    connection.Send("say " + currentchannel + " " + ourNickname + " " + text);
                } else {
                    ClientCommands[commandName](args);
                }
            } else {
                connection.Send("say " + currentchannel + " " + ourNickname + " " + text);
            }
        }

        //This simply makes sure that the chat window always scrolls down to the most recent messages.
        private void MessageBox_TextChanged(object sender, TextChangedEventArgs e) {
            MessageBox.ScrollToEnd();
        }
    }
}
