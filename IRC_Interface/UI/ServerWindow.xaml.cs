﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;

namespace IRC_Interface {
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window {
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


            Init();
        }

        private void killbutton_Click(object sender, RoutedEventArgs e) {
            //TODO: kill TCP gracefully and kick all clients well.
            connection.Dispose();
            App.Current.Shutdown();
        }


        public void Init() {
            InitCommands();

            connection = new TCPHandler();
            connection.OnMsg += OnMessage;
            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes)
                    PrintLine(m + ": Connected");
                else
                    PrintLine(m + ": Disconnected");
            };

            connection.CreateSocket(true);

            Room rootRoom = new Room("#root");
            rootRoom.MOTD = "Welcome to the server!";

            Rooms[rootRoom.Name] = rootRoom;
        }

        public void OnMessage(Socket soc, String message) {
            PrintLine((soc.RemoteEndPoint as IPEndPoint).Address + ": " + message);

            string[] cmdParts = message.Split(new char[] { ' ' }, 2);
            String commandName = cmdParts[0].Replace("\0", String.Empty).Trim();

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


        public void PrintLine(String text) {
            theTextArea.Inlines.Add(new Run(text));
        }
    }
}