using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IRC_Interface {
    /// <summary>
    /// A simple representation of a clinet for the server.
    /// </summary>
    public class User {
        public Socket connection { get; private set; }
        public String Nick { get; private set; }

        public List<Room> ConnectedRooms = new List<Room>();

        public User(Socket soc, String name) {
            connection = soc;
            Nick = name;

        }

        public void LeaveAllRooms() {
            while (ConnectedRooms.Count != 0)
                ConnectedRooms[0].RemoveUser(Nick);

            ConnectedRooms.Clear();
        }
       

        public void SendMsg(String msg) {
            connection.Send(Util.StoB(msg));
        }
    }
}
