using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Server
{
    public class GroupInviteTimeoutEventArgs: EventArgs
    {
        private List<LoggedClient> clients;
        public List<LoggedClient> Clients
        {
            get
            {
                return clients;
            }
            private set
            {
                clients = value;
            }
        }

        public GroupInviteTimeoutEventArgs(List<LoggedClient> clients)
        {
            Clients = clients;
        }
    }
}
