using System;
using System.Collections.Generic;
using System.Text;

namespace IRC_Interface {
    /// <summary>
    /// A simple representation of a channel for the server.
    /// </summary>
    public class Room {
        public String Name { get; private set; }
        public String MOTD;

        public Dictionary<String, User> ConnectedUsers = new Dictionary<String, User>();

        public Room(String name) {
            Name = name;
            MOTD = "Welcome to the channel: " + name;
        }

        /// <summary>
        /// retrives a comma delimited list of all users in this channel
        /// </summary>
        /// <returns>all connected users</returns>
        public String GetAllNicks() {
            String nicks = "";
            int i = 0;
            foreach (User u in ConnectedUsers.Values) {
                nicks += u.Nick + (++i < ConnectedUsers.Values.Count ? ", " : "");
            }

            return nicks;
        }

        public bool HasUser(String Nick) {
            return ConnectedUsers.ContainsKey(Nick);
        }
        public User GetUser(String Nick) {
            return ConnectedUsers[Nick];
        }

        /// <summary>
        /// Removes a user from the channel.
        /// It also send all other conneted users a "left" messaged.
        /// </summary>
        /// <param name="Nick">The name of the User to remove</param>
        /// <returns>The user that was removed or null on error</returns>
        public User RemoveUser(String Nick) {
            if (!HasUser(Nick)) {
                return null;
            } else {
                User temp = ConnectedUsers[Nick];
                temp.ConnectedRooms.Remove(this);

                ConnectedUsers.Remove(Nick);

                foreach (User u in ConnectedUsers.Values) {
                    if (u != temp)
                        u.SendMsg("left " + temp.Nick + " " + Name);
                }

                temp.SendMsg("left " + temp.Nick + " " + Name);
                return temp;
            }
        }

        /// <summary>
        /// Adds a user to this channel.
        /// It also sends all connected users a "joined" message
        /// </summary>
        /// <param name="newUser">The user to add.</param>
        public void AddUser(User newUser) {
            ConnectedUsers[newUser.Nick] = newUser;
            newUser.ConnectedRooms.Add(this);

            newUser.SendMsg("joined " + newUser.Nick + " " + Name);
            newUser.SendMsg("said " + Name + " " + Name + " " + MOTD);

            StringBuilder nicks = new StringBuilder();
            foreach (User u in ConnectedUsers.Values) {
                nicks.Append(u.Nick).Append(", ");
                if (u != newUser)
                    u.SendMsg("joined " + newUser.Nick + " " + Name);
            }
        }

        /// <summary>
        /// Broadcasts a message to all connected users
        /// </summary>
        /// <param name="SourceNick">Whom this message is from (can be channel name)</param>
        /// <param name="Message">The message</param>
        public void Say(String SourceNick, String Message) {
            foreach (User u in ConnectedUsers.Values) {
                u.SendMsg("said " + SourceNick + " " + Name + " " + Message);
            }
        }

    }
}
