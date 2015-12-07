using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace IRC_Interface {
    public enum ConnectionStatus { Succes, Failure};
    /// <summary>
    /// This class handles all of the actuall networking.
    /// </summary>
    public class TCPHandler: IDisposable {
        private const int MsgSize = 256;
        private Socket connection = null;

        private Thread Listner;

        //Fired when a message is recived
        public delegate void TCPMsgHandler(Socket recivingSocket, String messeage);
        public event TCPMsgHandler OnMsg;
        //Fired when connection state changes
        public delegate void TCPConnection(String IP, ConnectionStatus status);
        public event TCPConnection OnConnection;
        //Fired when connection is dead
        public delegate void TCPTerminated();
        public event TCPTerminated OnBadThing;

        ///Used to kill the connection grecefully.
        public bool KillConnection { get; set; } = false;
        public bool IsListining { get; set; } = false;
        public String Address { set; get; }
        public int Port { set; get; }

        public TCPHandler(String address = "localhost", int port = 1337) {
            Address = address;
            Port = port;
        }

        public void CreateSocket(bool IsHost) {
            if (!IsHost) {
                Connect();
            } else {
                Listen();
            }
        }

        /// <summary>
        /// Sends a message to the current connection
        /// </summary>
        public void Send(String data) {
            if (connection == null || !connection.Connected)
                throw new Exception("No Connection Established"); //TODO: add custom exception

            byte[] msg = Util.StoB(data);

            if (msg.Length > MsgSize)
                throw new Exception("Messeage too long"); //TODO: custom exception/splitting

            connection.Send(msg);
        }

        /// <summary>
        /// Tries to greacefully kill the socket
        /// </summary>
        public void Dispose() {
            KillConnection = true;
            try {
                connection.Shutdown(SocketShutdown.Both);
            } catch (Exception) {
                //This means that the connection is already terminated, so no real need to do anything.
            }
        }


        /// <summary>
        /// Tries to connect to a server, and then start a listing loop.
        /// </summary>
        private void Connect() {
            if (String.IsNullOrEmpty(Address)) return;

            IPAddress [] addresses;
            if (Address == "localhost")
                addresses = Dns.GetHostAddresses(Dns.GetHostName());
            else
                addresses = Dns.GetHostAddresses(Address);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress curAddr in addresses) {
                try {
                    IPEndPoint ipe = new IPEndPoint(curAddr, Port);
                    connection = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    connection.Connect(ipe);

                    //If we connect, start a simple listining loop.
                    if (connection.Connected) {
                        OnConnection?.Invoke(Address + ":" + Port, ConnectionStatus.Succes);

                        Listner = new Thread(() => {
                            while (true) {
                                byte[] buffer = new byte[MsgSize];
                                int size = 0;

                                try {
                                    size = connection.Receive(buffer);
                                } catch (SocketException) {
                                    KillConnection = true;
                                    size = 0;
                                }

                                if (size == 0 && KillConnection) {
                                    OnBadThing?.Invoke();
                                    connection.Close();
                                    connection.Dispose();
                                    break;
                                }
                                //connection.Send(buffer);
                                String temp = Util.BtoS(buffer);

                                OnMsg?.Invoke(connection, temp);

                                Thread.Sleep(100);
                            }
                        });
                        Listner.Start();
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
            OnConnection?.Invoke(Address + ":" + Port, ConnectionStatus.Failure);
            connection = null;

        }

        /// <summary>
        /// As a server, we will listen on the main socket 
        /// and spawn of other sockets for incomming connetions. 
        /// 
        /// Those connections will be maintined untill the die
        /// </summary>
        private void Listen() {
            IPAddress [] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            // Create a TCP/IP socket.

            foreach (IPAddress curAddr in addresses) {
                try {
                    IPEndPoint ipe = new IPEndPoint(curAddr, Port);
                    connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    connection.Bind(ipe);
                    connection.Blocking = false;
                    connection.Listen(10);

                    IsListining = true;
                    break;
                } catch (SocketException) {
                    //something mesed up during our connection attempt.
                    //Wont crash out of the loop since we might have more IPs to try
                    continue;
                }
            }

            if (!IsListining) {
                OnConnection?.Invoke("listen", ConnectionStatus.Failure);
            } else {
                OnConnection?.Invoke("listen", ConnectionStatus.Succes);

                Listner = new Thread(() => {
                    while (true) {
                        if (!KillConnection) {
                            try {
                                Socket tempListener = connection.Accept();
                                OnConnection?.Invoke(Address + ":" + Port, ConnectionStatus.Succes);

                                new Thread((soc) => {
                                    Socket tempSocket = soc as Socket;
                                    while (true) {

                                        byte[] buffer = new byte[MsgSize];
                                        if (KillConnection)
                                            tempSocket.Shutdown(SocketShutdown.Both);
                                        int size = 0;
                                        try {
                                            size = tempSocket.Receive(buffer);

                                            String temp = Util.BtoS(buffer);

                                            OnMsg?.Invoke(tempSocket, temp);
                                        } catch (SocketException e) {
                                            if (e.ErrorCode == (int)SocketError.ConnectionAborted || e.ErrorCode != (int)SocketError.WouldBlock) {
                                                KillConnection = true;
                                                size = 0;
                                            }
                                            
                                        } catch (ObjectDisposedException) {
                                            break;
                                        }

                                        if (size == 0 && KillConnection) {
                                            OnBadThing?.Invoke();
                                            tempSocket.Close();
                                            tempSocket.Dispose();
                                            break;
                                        }
                                        Thread.Sleep(100);
                                    }

                                    OnConnection?.Invoke(Address + ":" + Port, ConnectionStatus.Failure);

                                }).Start(tempListener);
                            } catch (SocketException e) {
                                //If the accept call has nothing to accept, this will do nothing
                                //All other errors are re-thrown.
                                if (e.ErrorCode != (int)SocketError.WouldBlock)
                                    throw e;
                            }
                        } else {
                            try {
                                connection.Shutdown(SocketShutdown.Both);
                            } catch (SocketException) {
                                //The socket was not connected/listening.
                            }
                            connection.Close();
                            connection.Dispose();
                            break;
                        }
                        Thread.Sleep(100);
                    }

                });
                Listner.Start();
            }
        }
    }
}
