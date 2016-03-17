using PKRY.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Server
{
    public class ClientsGroup
    {
        private List<LoggedClient> loggedClients;

        public ClientsGroup(List<LoggedClient> loggedClients, Dictionary<string, InviteResponse> inviteResponses)
        {
            this.loggedClients = loggedClients;
            SendMultiplicativeMessages(inviteResponses);
        }

        private void SendMultiplicativeMessages(Dictionary<string, InviteResponse> inviteResponses)
        {
            int clientsCount = loggedClients.Count;
            for (int i = 0; i < clientsCount; ++i)
            {
                var client = loggedClients[i];
                LoggedClient nextClient;
                if (i < clientsCount - 1)
                    nextClient = loggedClients[i + 1];
                else
                    nextClient = loggedClients.First();
                var clientMultiplicativeGroup = inviteResponses[client.Username].UsersMultiplicativeGroup;
                clientMultiplicativeGroup.Username = client.Username;
                var serizlizedClientMultiplicativeGroup = MessagesSerializer.Serialize(clientMultiplicativeGroup);
                var baseMessage = new BaseMessage(MessageType.MultiplicativeGroupNotifier, serizlizedClientMultiplicativeGroup);
                var serizedBaseMessage = MessagesSerializer.Serialize(baseMessage);
                try
                {
                    nextClient.SendMessage(serizedBaseMessage);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public bool GroupContainsUser(LoggedClient loggedClient)
        {
            return loggedClients.Contains(loggedClient);
        }

        public void HandleDHBigIntRequest(LoggedClient loggedClient, DHBigInt dhBigInt)
        {
            try
            {
                int clientIndex = loggedClients.IndexOf(loggedClient);
                dhBigInt.Username = loggedClient.Username;
                var serializedDHBigInt = MessagesSerializer.Serialize(dhBigInt);
                var baseMessage = new BaseMessage(MessageType.DHBigInt, serializedDHBigInt);
                var seriazlizedBaseMessage = MessagesSerializer.Serialize(baseMessage);
                if(clientIndex == 0)
                {
                    loggedClients[loggedClients.Count - 1].SendMessage(seriazlizedBaseMessage);
                }
                else
                {
                    loggedClients[clientIndex - 1].SendMessage(seriazlizedBaseMessage);
                }
            }
            catch { }
        }

        public void HandleRBigIntRequest(LoggedClient loggedClient, RBigInt rBigInt)
        {
            try
            {
                rBigInt.Username = loggedClient.Username;
                var serizlizedRBigInt = MessagesSerializer.Serialize(rBigInt);
                var baseMessage = new BaseMessage(MessageType.RBigInt, serizlizedRBigInt);
                var serizedBaseMessage = MessagesSerializer.Serialize(baseMessage);
                int clientsCount = loggedClients.Count;
                for (int i = 0; i < clientsCount; ++i)
                {
                    var client = loggedClients[i];
                    try
                    {
                        client.SendMessage(serizedBaseMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Wrong R");
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("R sended");
            }
            catch { }
        }

        public void HandleMessage(LoggedClient loggedClient, BaseMessage message)
        {
            try
            {
                var serizedBaseMessage = MessagesSerializer.Serialize(message);
                int clientsCount = loggedClients.Count;
                for (int i = 0; i < clientsCount; ++i)
                {
                    var client = loggedClients[i];
                    try
                    {
                        client.SendMessage(serizedBaseMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Couldn't send to client");
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("R sended");
            }
            catch { }
        }
    }
}
