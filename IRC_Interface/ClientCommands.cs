using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace IRC_Interface {
    /// <summary>
    /// This is an extention of the clientwindow class
    /// Here the mapping of commands (both server and client) to functions
    /// is defined. 
    /// 
    /// I chose to put it into a speparate file so as to not clutter the main one.
    /// </summary>
    public partial class ClientWindow {

        private void InitCommands() {
            //////////////////////
            // Server Commands
            //command template:
            //Commands[""] = (soc, args) => { };
            
            //These are the errors the server could return.
            
            //Happend when we try to connect with an existing nickname
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
                PrintLine("Spesified channel doesn't exist");
            };
            ServerCommands["no-user"] = (args) => {
                PrintLine("Spesified User doesn't exist");
            };
            ServerCommands["bad-command"] = (args) => {
                PrintLine("Bad Command, please try again.");
            };

            //commands that the server will send back to us.

            //This one is sent when someone says something in a channel.
            ServerCommands["said"] = (args) => {
                String srcNick = args[0];
                String chan = args[1];

                //This rebuild the message into a single string from a list of words.
                String msg = "";
                for (int i = 2; i < args.Length; i++) msg += (args[i] + " ");

                if (srcNick != chan)
                    PrintLine(srcNick + ": " + msg, chan);
                else
                    PrintLine(msg, chan);

                if (chan != currentchannel) {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in channelList.Items) {
                            if ((l.Content as String) == chan) {
                                l.Background = ColChanMsg;
                            }
                        }
                    }));
                }
            };

            //Sent when the server send back a list of users on a channel.
            ServerCommands["users"] = (args) => {
                String channel = args[0];
                String nicklist = "";

                Dispatcher.Invoke(new Action(() => {
                    for (int i = 1; i < args.Length; i++) {
                        NickList.Items.Add(args[i].Replace(",", String.Empty));
                        nicklist += args[i];
                    }
                }));

                PrintLine("Connected Users: " + nicklist, channel);
            };

            //Sent when a user joines a channel (including us).
            //This adds users to our userlist or handles moving ourselved into a new channel.
            ServerCommands["joined"] = (args) => {
                String nick = args[0];
                String channel = args[1];

                if (nick == ourNickname) {
                    Dispatcher.Invoke(new Action(() => {
                        channelList.Items.Add(new Label() { Content = channel });
                    }));
                    ChangeChatTarget(channel);

                } else if (nick != ourNickname && channel == currentchannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Add(nick);
                        PrintLine(nick + " joined " + channel, channel);
                    }));
                } else {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in channelList.Items) {
                            if ((l.Content as String) == channel) {
                                l.Background = ColChanChange;
                            }
                        }
                    }));
                }

            };

            //Sent when as reply to the nicklist command
            ServerCommands["nicks"] = (args) => {
                String room = args[0];
                if (room == currentchannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Clear();
                        for (int i = 1; i < args.Length; i++)
                            NickList.Items.Add(args[i].Replace(",",""));
                    }));
                }
            };

            //Send when somone leaves a channel (including ourselves);
            //If we leave the #root channel, we disconnect from the server and
            //close our connection.
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
                            foreach (Label l in channelList.Items) {
                                if ((l.Content as String) == channel) {
                                    removeTar = l;
                                    break;
                                }
                            }
                            channelList.Items.Remove(removeTar);
                        }));
                        PrintLine("You left " + channel);
                    }
                } else if (nick != ourNickname && channel == currentchannel) {
                    Dispatcher.Invoke(new Action(() => {
                        NickList.Items.Remove(nick);
                        PrintLine(nick + " left " + channel, channel);
                    }));
                } else {
                    Dispatcher.Invoke(new Action(() => {
                        foreach (Label l in channelList.Items) {
                            if ((l.Content as String) == channel) {
                                l.Background = ColChanChange;
                            }
                        }
                    }));
                }
            };

            ClientCommands["pong"] = (args) => {
                //do nothing.
            };

            //////////////////////
            // Client Commands

            //These are interpreted from the chat box and used to do special things.

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
               connection.Send("leave " + ourNickname + " " + currentchannel);
            };


            ClientCommands["disconnect"] = (args) => {
                connection.Send("leave " + ourNickname + " #root");
            };

            ClientCommands["nicklist"] = (args) => {
                connection.Send("nicklist " + currentchannel);
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
