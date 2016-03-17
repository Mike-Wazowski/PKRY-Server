using PKRY.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Server
{
    public class GroupCreatedEventArgs: EventArgs
    {
        private List<LoggedClient> loggedClients;
        public List<LoggedClient> LoggedClients
        {
            get
            {
                return loggedClients;
            }
            private set
            {
                loggedClients = value;
            }
        }

        private Dictionary<string, InviteResponse> inviteRosponses;
        public Dictionary<string, InviteResponse> InviteRosponses
        {
            get
            {
                return inviteRosponses;
            }
            private set
            {
                inviteRosponses = value;
            }
        }

        public GroupCreatedEventArgs(List<LoggedClient> loggedClients, Dictionary<string, InviteResponse> inviteResponses)
        {
            LoggedClients = loggedClients;
            InviteRosponses = inviteResponses;
        }
    }
}
