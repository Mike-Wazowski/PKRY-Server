using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKRY.Messages;
using System.Timers;

namespace PKRY.Server
{
    public class ClientsGroupInvite
    {
        private Timer timer;
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

        public event EventHandler<GroupInviteTimeoutEventArgs> GroupInviteTimeout;
        public event EventHandler<GroupCreatedEventArgs> GroupCreated;

        public ClientsGroupInvite(List<LoggedClient> groupClients)
        {
            timer = new Timer();
            timer.Interval = 300000;
            timer.Elapsed += Timer_Elapsed;
            var orderedList = groupClients.OrderBy(x => x.Username).ToList();
            LoggedClients = orderedList;
            inviteRosponses = new Dictionary<string, InviteResponse>();
            InviteUsers();
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            var groupInviteTimeoutEvent = GroupInviteTimeout;
            if(groupInviteTimeoutEvent != null)
            {
                groupInviteTimeoutEvent(this, new GroupInviteTimeoutEventArgs(LoggedClients));
            }
        }

        private void InviteUsers()
        {
            if (LoggedClients != null)
            {
                var usernames = LoggedClients.Select(x => x.Username).ToList();
                var inviteMessage = new StatusNotifier(true, usernames);
                inviteMessage.GroupSize = usernames.Count;
                var serializedInviteMessage = MessagesSerializer.Serialize(inviteMessage);
                var baseMessage = new BaseMessage(MessageType.StatusNotifier, serializedInviteMessage);
                var seriazlizedBaseMessage = MessagesSerializer.Serialize(baseMessage);
                for (int i = 0; i < LoggedClients.Count; ++i)
                {
                    var loggedClient = LoggedClients[i];
                    try
                    {
                        loggedClient.SendMessage(seriazlizedBaseMessage);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void AddInviteResponse(string username, InviteResponse inviteResponse)
        {
            if (inviteResponse.IsAccepted == true)
            {
                if (inviteRosponses.ContainsKey(username))
                    inviteRosponses[username] = inviteResponse;
                else
                    inviteRosponses.Add(username, inviteResponse);
                if (inviteRosponses.Count == LoggedClients.Count)
                {
                    var groupCreatedEvent = GroupCreated;
                    if (groupCreatedEvent != null)
                    {
                        groupCreatedEvent(this, new GroupCreatedEventArgs(LoggedClients, inviteRosponses));
                    }
                }
            }
        }
    }
}
