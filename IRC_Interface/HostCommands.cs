using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRC_Interface {
    public partial class ServerWindow {

        private void InitCommands() {
            //Commands[""] = (soc, args) => { };
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

            Commands["nicklist"] = (soc, args) => {
                String room = args[0];
                if (Rooms.ContainsKey(room)) {
                    soc.Send(Util.StoB("nicks " + room + " " + Rooms[room].GetAllNicks()));
                } else {
                    soc.Send(Util.StoB(Errors.NoChan));
                }
            };

            Commands["roomlist"] = (soc, args) => {
                String rooms = "";
                int i = 0;
                foreach (Room r in Rooms.Values)
                    rooms += r.Name + (++i < Rooms.Values.Count ? ", " : "");

               soc.Send(Util.StoB("said #root #root " + rooms));
            };

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
            Commands["leave"] = (soc, args) => {
                String nick = args[0];
                String roomName = args[1];
                if (roomName == "#root") {
                    if (Rooms["#root"].HasUser(nick)) {
                        var userRooms = Rooms["#root"].ConnectedUsers[nick].ConnectedRooms;
                        foreach (Room r in userRooms) {
                            r.RemoveUser(nick);
                        }
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
                } else {
                    //TODO: handle private messeages here
                }
            };

            Commands["ping"] = (soc, args) => { soc.Send(Util.StoB("pong")); };
        }
    }
}
