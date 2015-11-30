using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    public class Client {
        private static Object theLock = new Object();
        TCPHandler connection;
        private bool IsConnected = false;

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

            if (String.IsNullOrEmpty(address))
                connection = new TCPHandler();
            else if (String.IsNullOrEmpty(port))
                connection = new TCPHandler(address);

            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes) {
                    IsConnected = true;
                    Util.Print(ConsoleColor.Green, "Connected");
                } else
                    Util.Print(ConsoleColor.Red, "Disconnected");
            };
            connection.OnMsg += (s, msg) => {
                //lock (theLock) {
                    Util.Print(msg);
                //}
            };

            connection.CreateSocket(false);

            new Thread(() => {

                while (!IsConnected) {
                    Thread.Sleep(500);
                }
                connection.Send("connect sabreman");
                Thread.Sleep(3000);
                connection.Send("ping");
                Thread.Sleep(3000);

                while (true) {
                    Util.Print("MSG: ");
                    String msg = Console.ReadLine();

                    if (msg == "EXIT") break;
                    connection.Send(msg);

                    Thread.Sleep(1000);
                }
            }).Start();
        }

        public void Draw() { 

        }
    }
}
