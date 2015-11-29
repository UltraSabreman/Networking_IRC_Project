using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    class Client {
        TCPHandler connection;

        public Client() {
            Console.Clear();
            Util.PrintLine("Mode: ", ConsoleColor.Green, "CLIENT");

            Init();
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

            connection = new TCPHandler(address, int.Parse(port), false);

            while (true) {
                Util.Print("MSG: ");
                String msg = Console.ReadLine();
                if (msg == "EXIT") break;
                connection.Send(msg);
            }
        }

        public void Draw() { 

        }
    }
}
