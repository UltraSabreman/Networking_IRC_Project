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
            else
                connection = new TCPHandler(address, int.Parse(port));

            Util.PrintLine("YAY 1");

            connection.OnConnection += (m, s) => {
                if (s == ConnectionStatus.Succes) {
                    IsConnected = true;
                    Util.Print(ConsoleColor.Green, "Connected");
                } else
                    Util.Print(ConsoleColor.Red, "Disconnected");
            };
            Util.PrintLine("YAY 2");

            connection.OnMsg += (s, msg) => {
                //lock (theLock) {
                    Util.Print(msg);
                //}
            };
            Util.PrintLine("YAY 3");

            connection.CreateSocket(false);
            Util.PrintLine("YAY 4");

            new Thread(() => {
                Util.PrintLine("YAY 5");

                while (!IsConnected) {
                    Thread.Sleep(500);
                }
                Util.PrintLine("YAY 6");

                connection.Send("connect sabreman");
                Util.PrintLine("YAY 7");

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
