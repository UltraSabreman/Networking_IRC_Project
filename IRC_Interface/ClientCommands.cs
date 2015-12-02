using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace IRC_Interface {
    public partial class ClientWindow {

        private void InitCommands() {
            //////////////////////
            // Server Commands
            //command template:
            //Commands[""] = (soc, args) => { };
            ServerCommands["said"] = (args) => {
                String srcNick = args[0];
                String chan = args[1];
                String msg = "";
                for (int i = 2; i < args.Length; i++) msg += (args[i] + " ");

                //TODO: private messeageing
                PrintLine(srcNick + ": " + msg, chan);
                if (chan != currentChannel) {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in ChannelList.Items) {
                            if ((l.Content as String) == chan) {
                                l.Background = ColChanMsg;
                            }
                        }
                    }));
                }
            };

            ServerCommands["users"] = (args) => {
                String channel = args[0];
                String nicklist = "";

                Dispatcher.Invoke(new Action(() => {
                    for (int i = 1; i < args.Length; i++) {
                        NickList.Items.Add(args[i].Replace(",", String.Empty));
                        nicklist += args[i];
                    }
                }));

                PrintLine("Connected Users: " + nicklist);
            };

            ServerCommands["joined"] = (args) => {
                String nick = args[0];
                String chanel = args[1];

                if (nick == ourNickname) {
                    Dispatcher.Invoke(new Action(() => {
                        ChannelList.Items.Add(new Label() { Content = chanel });
                    }));
                    ChangeChatTarget(chanel);

                } else if (nick != ourNickname && chanel == currentChannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Add(nick);
                        PrintLine(nick + " joined " + chanel);
                    }));
                } else {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in ChannelList.Items) {
                            if ((l.Content as String) == chanel) {
                                l.Background = ColChanChange;
                            }
                        }
                    }));
                }

            };

            ServerCommands["nicks"] = (args) => {
                String room = args[0];
                if (room == currentChannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Clear();
                        for (int i = 1; i < args.Length; i++)
                            NickList.Items.Add(args[i].Replace(",",""));
                    }));
                }
            };

            ServerCommands["left"] = (args) => {
                String chanel = args[0];
                String nick = args[1];

                if (nick == ourNickname) {
                    ChangeChatTarget("#root");
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in ChannelList.Items) {
                            if ((l.Content as String) == chanel)
                                ChannelList.Items.Remove(l);
                        }
                    }));
                    PrintLine("You left " + chanel);
                } else if (nick != ourNickname && chanel == currentChannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Remove(nick);
                        PrintLine(nick + " left " + chanel);
                    }));
                } else {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in ChannelList.Items) {
                            if ((l.Content as String) == chanel) {
                                l.Background = ColChanChange;
                            }
                        }
                    }));
                }
            };

            //////////////////////
            // Client Commands

            ClientCommands["join"] = (args) => {
                String roomName = args[0];
                if (!roomName.StartsWith("#")) {
                    PrintLine("ERROR: Room must start with '#'");
                } else {
                    connection.Send("join " + ourNickname + " " + roomName);
                }
            };

            ClientCommands["leave"] = (args) => {
               connection.Send("leave " + ourNickname + " " + currentChannel);
            };


            ClientCommands["disconnect"] = (args) => {
                connection.Send("leave " + ourNickname + " #root");
            };

            ClientCommands["nicklist"] = (args) => {
                connection.Send("nicklist " + currentChannel);
            };

            ClientCommands["roomlist"] = (args) => {
                connection.Send("roomlist");
            };

        }
    }
}
