using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    class Host {
        TCPHandler connection;

        public Host() {
            Console.Clear();
            Util.PrintLine("Mode: ", ConsoleColor.Red, "SERVER");

            Init();
        }

        public void Init() {
            connection = new TCPHandler(true);

            
            connection.OnMsg += OnMessage;
        }

        public void OnMessage(String message) {
            Util.PrintLine("MSG: " + message);
        }
    }
}
