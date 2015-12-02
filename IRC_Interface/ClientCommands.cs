using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRC_Interface {
    public partial class ClientWindow {

        private void InitCommands() {
            //Commands[""] = (soc, args) => { };
            Commands["said"] = (soc, args) => {
                String srcNick = args[0];
                String chan = args[1];
                String msg = "";
                for (int i = 2; i < args.Length; i++) msg += args[i];

                //TODO: private messeageing
                //TODO: some sort of notifications of messeages not on our channel
                if (chan == currentChannel) {
                    PrintLine(srcNick + ": " + msg);
                }
            };

            Commands["joined"] = (soc, args) => {
                ChangeChatTarget(args[0]);
                ChannelList.Items.Add(args[0]);
            };

            Commands["left"] = (soc, args) => {
                throw new NotImplementedException();
            };
        }
    }
}
