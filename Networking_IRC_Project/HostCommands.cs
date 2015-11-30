using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Networking_IRC_Project {
    public partial class Host {

        private void InitCommands() {
            //Commands["connect"] = (soc, args) => { }, command template
            Commands["connect"] = (soc, args) => {
                if (Rooms["#root"].HasUser(args[0])) {
                    soc.Send(Util.StoB(Errors.NickExists));
                    soc.Shutdown(SocketShutdown.Both);
                    soc.Close();
                    return;
                }
                User tempUser = new User(soc, args[0]);
                Rooms["#root"].AddUser(tempUser);

            };
            Commands["ping"] = (soc, args) => { soc.Send(Util.StoB("pong")); };
        }
    }
}
