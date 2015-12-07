using System;
using System.Net.Sockets;

namespace IRC_Interface {
    /// <summary>
    /// This partial class holds the definitions for all server commands
    /// </summary>
    public partial class ServerWindow {

        private void InitCommands() {
            //Commands[""] = (soc, args) => { };

            //Recived when a client first connects. 
            //this checks to see if they can and adds them to the #root channel.
            Commands["connect"] = (soc, args) => {
                String newNick = args[0];
                if (Rooms["#root"].HasUser(newNick)) {
                    soc.Send(Util.StoB(Errors.NickExists));
                    soc.Shutdown(SocketShutdown.Both);
                    soc.Close();
                } else {
                    User tempUser = new User(soc, newNick);
                    Rooms["#root"].AddUser(tempUser);
                }

            };

            //Recived when a user whats a list of connected users to a channel.
            Commands["nicklist"] = (soc, args) => {
                String room = args[0];
                if (Rooms.ContainsKey(room)) {
                    soc.Send(Util.StoB("nicks " + room + " " + Rooms[room].GetAllNicks()));
                } else {
                    soc.Send(Util.StoB(Errors.NoChan));
                }
            };

            //Recived when a user whants all rooms on the server
            Commands["roomlist"] = (soc, args) => {
                String rooms = "";
                int i = 0;
                foreach (Room r in Rooms.Values)
                    rooms += r.Name + (++i < Rooms.Values.Count ? ", " : "");

               soc.Send(Util.StoB("said #root #root " + rooms));
            };

            //Recived when a user is tring to join a room.
            //Will create a room if it doesnt exist.
            Commands["join"] = (soc, args) => {
                String nick = args[0];
                String roomName = args[1];
                if (Rooms["#root"].HasUser(nick)) {
                    User tarUser = Rooms["#root"].ConnectedUsers[nick];
                    if (Rooms.ContainsKey(roomName)) {
                        Rooms[roomName].AddUser(tarUser);
                    } else {
                        Rooms[roomName] = new Room(roomName);
                        Rooms[roomName].AddUser(tarUser);
                    }
                } else {
                    soc.Send(Util.StoB(Errors.NoUser));
                }
            };

            //Recived when a user is trying to leave a room.
            //Will terminate the connection if the user leaves  #root
            Commands["leave"] = (soc, args) => {
                String nick = args[0];
                String roomName = args[1];
                if (roomName == "#root") {
                    if (Rooms["#root"].HasUser(nick)) {
                        Rooms["#root"].GetUser(nick).LeaveAllRooms();

                        soc.Shutdown(SocketShutdown.Both);
                        soc.Close();
                    } else {
                        soc.Send(Util.StoB(Errors.NoUser));
                    }
                } else {
                    if (Rooms.ContainsKey(roomName)) {
                        if (Rooms[roomName].HasUser(nick)) {
                            Rooms[roomName].RemoveUser(nick);
                        } else {
                            soc.Send(Util.StoB(Errors.NoUser));
                        }
                    } else {
                        soc.Send(Util.StoB(Errors.NoChan));
                    }
                }

            };

            //Recived when a user sends a message
            Commands["say"] = (soc, args) => {
                String target = args[0];
                String src = args[1];
                String msg = "";
                for (int i = 2; i < args.Length; i++) msg += (args[i] + " ");
                if (target.StartsWith("#")) {
                    if (Rooms.ContainsKey(target)) {
                        Rooms[target].Say(src, msg);
                    } else {
                        soc.Send(Util.StoB(Errors.NoChan));
                    }
                }
            };

            Commands["ping"] = (soc, args) => { soc.Send(Util.StoB("pong")); };
        }
    }
}
