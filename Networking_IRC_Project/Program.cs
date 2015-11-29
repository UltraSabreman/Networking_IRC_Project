using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    class Program {
        private static Host host;
        private static Client client;

        static void Main(string[] args) {
            Util.Print("Is this a (s)erver or a (c)lient: ");
            if (Console.ReadKey().Key == ConsoleKey.S) {
                host = new Host();
            } else {
                client = new Client();
            }

        }
    }
}
