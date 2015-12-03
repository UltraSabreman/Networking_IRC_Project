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
            
            //return errors
            ServerCommands["nick-exists"] = (args) => {
                var result = System.Windows.MessageBox.Show("The nickname you chose already exists, please use another one.\nClick OK to try again or Cancel to quit.", "Bad nickname", System.Windows.MessageBoxButton.OKCancel);
                if (result == System.Windows.MessageBoxResult.OK) {
                    Dispatcher.Invoke(new Action(() => {
                        new InitWindow().Show();
                        Close();
                    }));
                }
                connection.Dispose();
            };

            ServerCommands["no-chan"] = (args) => {
                PrintLine("Spesified Channel doesn't exist");
            };
            ServerCommands["no-user"] = (args) => {
                PrintLine("Spesified User doesn't exist");
            };
            ServerCommands["bad-command"] = (args) => {
                PrintLine("Bad Command, please try again.");
            };

            //commands
            ServerCommands["said"] = (args) => {
                String srcNick = args[0];
                String chan = args[1];
                String msg = "";
                for (int i = 2; i < args.Length; i++) msg += (args[i] + " ");

                //TODO: private messeageing
                if (srcNick != chan)
                    PrintLine(srcNick + ": " + msg, chan);
                else
                    PrintLine(msg, chan);

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
                String nick = args[0];
                String channel = args[1];

                if (nick == ourNickname) {
                    if (channel == "#root") {
                        connection.Dispose();
                        return;
                    } else {
                        ChangeChatTarget("#root");
                        Dispatcher.Invoke(new Action(() => {
                            Object removeTar = null;
                            foreach (Label l in ChannelList.Items) {
                                if ((l.Content as String) == channel) {
                                    removeTar = l;
                                    break;
                                }
                            }
                            ChannelList.Items.Remove(removeTar);
                        }));
                        PrintLine("You left " + channel);
                    }
                } else if (nick != ourNickname && channel == currentChannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Remove(nick);
                        PrintLine(nick + " left " + channel);
                    }));
                } else {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in ChannelList.Items) {
                            if ((l.Content as String) == channel) {
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

            ClientCommands["multisay"] = (args) => {
                var roomsToMsg = new List<String>();
                String msg = "";
                foreach (String s in args) {
                    if (s.StartsWith("#"))
                        roomsToMsg.Add(s);
                    else
                        msg += s + " ";
                }

                foreach (String room in roomsToMsg)
                    connection.Send("say " + room + " " + ourNickname + " " + msg);
            };

            ClientCommands["multijoin"] = (args) => {
                foreach (String room in args) {
                    connection.Send("join " + ourNickname + " " + room);
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

            ClientCommands["help"] = (args) => {
                PrintLine("/help: this info dump");
                PrintLine("/join #<channelname>: Join a spesified channel (must start with '#'). If the channel doesn't exist, its automatically created.");
                PrintLine("/leave: makes you leave the current selected channel. Leaving the #root channel disconnects you from the server");
                PrintLine("/nicklist: manually update the list of users in this room (displayed on the right)");
                PrintLine("/roomlist: prints a list of all rooms on the server to the #root channel");
                PrintLine("/multisay <channels list> <message>: provided a space-delimited list of channel names, this will send the provided message to all of them");
                PrintLine("/multijoin <channel list>: provided a space-delimited list of channel names, this will join (or create) all of them");
            };

        }
    }
}
