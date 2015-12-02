using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC_Interface {
    //TODO: impliment permissions and modes
    //enum RoomMode { Public, Private, AdminPublic, Admin}
    //enum UserMode { Normal, LocalAdmin, Admin}
    public class Room {
        //MUST START WITH '#'
        public String Name { get; private set; }
        public String MOTD;
        //public RoomMode Mode;

        private Dictionary<String, User> ConnectedUsers = new Dictionary<String, User>();


        public Room(String name) {
            Name = name;
        }

        public bool HasUser(String Nick) {
            return ConnectedUsers.ContainsKey(Nick);
        }

        public void AddUser(User newUser) {
            ConnectedUsers[newUser.Nick] = newUser;

            newUser.SendMsg("joined " + Name);
            newUser.SendMsg("said " + Name + " " + Name + " " + MOTD);

            StringBuilder nicks = new StringBuilder();
            foreach (User u in ConnectedUsers.Values)
                nicks.Append(u.Nick).Append(", ");

            newUser.SendMsg("said " + Name + " " + Name + " List Users: " + nicks);
        }

        public void Say(String SourceNick, String Message) {
            foreach (User u in ConnectedUsers.Values) {
                u.SendMsg("said " + SourceNick + " " + Name + " " + Message);
            }
        }

    }
}
