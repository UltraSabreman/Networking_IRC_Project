using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Networking_IRC_Project {
    class TCPHandler: IDisposable {
        private Socket connection = null;
        public delegate void TCPMsgHandler(String messeage);
        public event TCPMsgHandler OnMsg;

        public bool KillConnection { get; set; } = false;
        public String Address { set; get; }
        public int Port { set; get; }

        public TCPHandler(bool IsHost = false) : this("localhost", 1337, IsHost) { }
        
        public TCPHandler(String address, int port, bool IsHost) {
            Address = address;
            Port = port;

            Connect(IsHost);
            if (IsHost) {
                Listen();
            }
        }


        public void Connect(bool IsHost = false) {
            if (IsHost) {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                // Create a TCP/IP socket.
 
                foreach (IPAddress curAddr in ipHostInfo.AddressList) {
                    try {
                        IPEndPoint ipe = new IPEndPoint(curAddr, Port);
                        connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        connection.Bind(ipe);
                        //connection.Blocking = true;
                        connection.Listen(10);

                        return;
                    } catch (SocketException) {
                        //something mesed up during our connection attempt.
                        //Wont crash out of the loop since we might have more IPs to try
                        continue;
                    }
                }
            } else {
                if (String.IsNullOrEmpty(Address)) return;

                IPHostEntry hostEntry;
                if (Address == "localhost")
                    hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                else
                    hostEntry = Dns.GetHostEntry(Address);

                // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
                // an exception that occurs when the host IP Address is not compatible with the address family
                // (typical in the IPv6 case).
                foreach (IPAddress curAddr in hostEntry.AddressList) {
                    try {
                        IPEndPoint ipe = new IPEndPoint(curAddr, Port);
                        connection = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        connection.Connect(ipe);

                        if (connection.Connected) {
                            return;
                        } else {
                            continue;
                        }
                    } catch (SocketException) {
                        //something mesed up during our connection attempt.
                        //Wont crash out of the loop since we might have more IPs to try
                        continue;
                    }
                }
                connection = null;
            }
        }
   
        /// <summary>
        /// This class only supports sending strings.
        /// </summary>
        public void Send(String data) {
            if (connection == null || !connection.Connected)
                throw new Exception("No Connection Established"); //TODO: add custom exception

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] msg = encoding.GetBytes(data);

            if (msg.Length > 256)
                throw new Exception("Messeage too long"); //TODO: custom exception/splitting

            connection.Send(msg);
                
        }

        private void Listen() {
            new Thread(() => {
                Socket tempListener = connection.Accept();
                while (!KillConnection) {
                    try {

                        byte[] buffer = new byte[256];
                        int size = tempListener.Receive(buffer);
                        String temp = Encoding.UTF8.GetString(buffer, 0, size);

                        OnMsg?.Invoke(temp);
                    } catch (SocketException e) {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        public void Dispose() {
            KillConnection = true;
            connection.Shutdown(SocketShutdown.Both);
            connection.Close();
            connection.Dispose();
        }
    }
}
