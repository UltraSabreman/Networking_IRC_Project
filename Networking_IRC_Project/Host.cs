using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    public partial class Host {
        TCPHandler connection;
        private Dictionary<String, Room> Rooms = new Dictionary<String, Room>();
        private Dictionary<String, Action<Socket, String[]>> Commands = new Dictionary<String, Action<Socket, String[]>>();
        //private Regex commandSplitter = new Regex("(?:\\\"([^\"]*)\\\"|([^ ]*))(?: ?)", RegexOptions.Compiled);


        public Host() {
            Console.Clear();
            Util.PrintLine("Mode: ", ConsoleColor.Red, "SERVER");

            Init();
        }

        public void Init() {
            InitCommands();

            connection = new TCPHandler();
            connection.OnMsg += OnMessage;
            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes)
                    Util.PrintLine(m + ": ", ConsoleColor.Green, "Connected");
                else
                    Util.PrintLine(m + ": ", ConsoleColor.Red, "Disconnected");
            };

            connection.CreateSocket(true);

            Room rootRoom = new Room("#root");
            rootRoom.MOTD = "Welcome to the server!";

            Rooms[rootRoom.Name] = rootRoom;
        }

        public void OnMessage(Socket soc, String message) {
            Util.PrintLine((soc.RemoteEndPoint as IPEndPoint).Address + ": " + message);
            string [] cmdParts = message.Split(new char [] { ' ' }, 2);
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
    }
}
