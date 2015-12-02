using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace IRC_Interface {
    public class User {
        public Socket connection { get; private set; }

        public String Nick { get; private set; }
        public String RealName { get; private set; }
        public String Email { get; private set; }

        //private bool IsAdmin;

        public List<Room> ConnectedRooms = new List<Room>();

        public User(Socket soc, String name) {
            connection = soc;
            Nick = name;

        }

       

        public void SendMsg(String msg) {
            //TODO: error handling
            connection.Send(Util.StoB(msg));
        }
        
    }
}
