using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PKRY.Messages;
using PKRY.Security;
using PKRY.Database;

namespace PKRY.Server
{
    public class Server
    {
        private TcpListener tcpListener;
        private bool running;

        private List<LoggedClient> loggedClients;
        private List<ClientsGroupInvite> clientsGroupInvites;
        private List<ClientsGroup> clientsGroups;
        private db_Entities context;
        private PasswordManager passwordManager;
        private List<User> databaseCachedUsers;

        private static readonly object syncRootAddUser = new object();
        private static readonly object syncRootClients = new object();
        private static readonly object syncRootGroupsInvites = new object();
        private static readonly object syncRootGroups = new object();

        public Server(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            running = false;
            loggedClients = new List<LoggedClient>();
            clientsGroupInvites = new List<ClientsGroupInvite>();
            clientsGroups = new List<ClientsGroup>();
            context = new db_Entities();
            passwordManager = new PasswordManager();
			//production
            //databaseCachedUsers = context.User.ToList();
			
			//dev
            databaseCachedUsers = new List<User> { new User { user_name = "J.Sobczynski", hash = "81202ff50604884098352312c8acae47b56ad0051a49a7a3887e7df7b4736ac5", salt = "XS4laZSRWRCltVQJ92Cf8qeh7SXuBXITZtj0neaTT+PR3yRyMVaaZDbyz/YJyDnF9MvmhOuptziPvvmHIOf6zA==" },
                new User { user_name = "B.Ostrowski", hash = "928bbdc09ae572df2b63b84211283b5a54f1aa723b1efe804c81a3be79539bac", salt = "VF5r3I5B2tto8LypXl7/LzoTQZa9IRACCLfxlq6m0xWY1NlC0UXqAr9sjiH5VkscJt8glVOlz4y5T35dTW0O4g==" },
                new User() {user_name = "R.Kowalski", hash = "84e8a9e5cbd1d77ef210e09aba9c678a4a8fc78ee6524881a974c588d29448fc", salt = "O81K98VFZvbbYhNEmx5e2ZBqVYt4S1URL+9SC+zkMGtB9eGXgQXcPgQL3P9IwPaItQUdfljWzx7DOnUZ31zKlA==" },
                new User() {user_name = "J.Banka", hash = "747faf54336ff4130896c99dfb1c47644241e6c66d75736a6421573f279922a6", salt = "rT3l5GG23e5mdi7ZFQN0byuP+0fUgwU60JPSe0lVfe69l3w8ong4oDrs7vj33in2qoNNZGVcR2t1MPE+HhbYhw==" },
                new User() {user_name = "Piotr" },new User() {user_name = "Kamil" } };
        }

        public void Start()
        {
            tcpListener.Start();
            running = true;
            while (running)
            {
                var client = tcpListener.AcceptTcpClient();
                var clientThread = new Thread(new ParameterizedThreadStart(HandleClientRequest));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }

        public void AddUser(string username, string password)
        {
            lock(syncRootAddUser)
            {
                var user = databaseCachedUsers.Where(u => u.user_name == username).FirstOrDefault();
                if (user == null)
                {
                    user = new User() { user_name = username };
                    string hashedPassword = passwordManager.EncryptPassword(password);
                    string salt = passwordManager.GenerateSalt();
                    user.hash = passwordManager.EncryptPassword(hashedPassword, salt);
                    user.salt = salt;
                    context.User.Add(user);
                    context.SaveChanges();
                    databaseCachedUsers = context.User.ToList();
                }
            }
        }

        private void HandleClientRequest(object obj)
        {
            Console.WriteLine("Client connected!");
            var tcpClient = (TcpClient)obj;
            var clientStream = tcpClient.GetStream();
            var reader = new StreamReader(clientStream);
            var writer = new StreamWriter(clientStream);
            LoggedClient loggedClient = null;
            try
            {
                var registerMessage = GetRegisterMessage(reader);
                var AESKey = AESWrapper.GetRandomBytes();
                var registerResponse = GetRegisterResponse(registerMessage, AESKey);
                writer.WriteLine(registerResponse);
                writer.Flush();
                Console.WriteLine("Client registered!");
                loggedClient = AuthorizeUser(tcpClient, AESKey);
                while(running)
                {
                    Console.WriteLine("While true...");


                    var input = reader.ReadLine();
                    if (input == null)
                    {
                        throw new ArgumentNullException("Null input");
                    }
                    loggedClient.HandleIncomingRequests(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            Console.WriteLine("logging out...");
            LogOutClient(loggedClient);
        }

        private void LogOutClient(LoggedClient loggedClient)
        {
            if (loggedClient != null)
            {
                Console.WriteLine("client not null...");
                try
                {
                    loggedClients.Remove(loggedClient);
                    Console.WriteLine("Client logged out");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Client not logged out...");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private LoggedClient AuthorizeUser(TcpClient tcpClient, byte[] AESKey)
        {
            var clientStream = tcpClient.GetStream();
            var reader = new StreamReader(clientStream);
            var writer = new StreamWriter(clientStream);
            bool isAuthorized = false;
            string loginResponse = String.Empty;
            while (!isAuthorized)
            {
                var loginMessage = GetLoginMessage(reader, AESKey);
                Console.WriteLine("Decrypt login message");
                if (UsernameAndPasswordMatch(loginMessage))
                {
                    Console.WriteLine("Password valid");
                    isAuthorized = true;
                    loginResponse = GetLoginResponse(true, loginMessage.Username, AESKey);
                    writer.WriteLine(loginResponse);
                    writer.Flush();
                    return AddClient(loginMessage.Username, AESKey, tcpClient);
                }
                else
                {
                    Console.WriteLine("Password invalid");
                    loginResponse = GetLoginResponse(false, loginMessage.Username, AESKey);
                }
                writer.WriteLine(loginResponse);
                writer.Flush();
            }
            return null;
        }

        private LoggedClient AddClient(string username, byte[] AESKey, TcpClient tcpClient)
        {
            lock(syncRootClients)
            {
                var clients = loggedClients;
                if (clients != null)
                {
                    var clientExist = clients.Where(c => c.Username == username).FirstOrDefault();
                    if(clientExist == null)
                    {
                        var loggedClient = new LoggedClient(username, tcpClient, AESKey);
                        loggedClient.MessageReceived += LoggedClient_MessageReceived;
                        loggedClients.Add(loggedClient);
                        NotifyNewClientHasConnected(loggedClient.Username);
                        return loggedClient;
                    }
                }
                return null;
            }
        }

        private void NotifyNewClientHasConnected(string newClientUsername)
        {
            var allLoggedClients = loggedClients;
            if(allLoggedClients != null)
            {
                for (int i = 0; i < allLoggedClients.Count; ++i)
                {
                    var me = allLoggedClients[i];
                    var myUsername = me.Username;
                    if (myUsername != newClientUsername)
                    {
                        var myFriends = allLoggedClients.Where(x => x.Username != myUsername).Select(u => u.Username).ToList();
                        var statusNotifier = new StatusNotifier(false, myFriends);
                        var serializedStatusNotifier = MessagesSerializer.Serialize(statusNotifier);
                        var baseMessage = new BaseMessage(MessageType.StatusNotifier, serializedStatusNotifier);
                        var serializedBaseMessage = MessagesSerializer.Serialize(baseMessage);
                        try
                        {
                            me.SendMessage(serializedBaseMessage);
                        }
                        catch { }
                    }
                }
            }
        }

        private void LoggedClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
#warning finnish it!!!
            var loggedClient = (LoggedClient)sender;
            var baseMessage = e.Message;
            switch(baseMessage.Type)
            {
                case MessageType.Message:
                    HandleMessage(loggedClient, baseMessage);
                    break;
                case MessageType.StatusNotifier:
                    InviteUsersToGroup(loggedClient, baseMessage);
                    break;
                case MessageType.InviteResponse:
                    HandleInviteResponse(loggedClient, baseMessage);
                    break;
                case MessageType.DHBigInt:
                    HandleDHBigIntMessage(loggedClient, baseMessage);
                    break;
                case MessageType.RBigInt:
                    HandleRBigInt(loggedClient, baseMessage);
                    break;
                case MessageType.LeaveGroup:
                    break;
                case MessageType.Logout:
                    throw new Exception("Client logged out");
                default:
                    break;
            }
        }

        private void HandleMessage(LoggedClient loggedClient, BaseMessage baseMessage)
        {
            try
            {
                var allGroups = clientsGroups;
                if (allGroups != null)
                {
                    var clientGroup = allGroups.Where(x => x.GroupContainsUser(loggedClient)).FirstOrDefault();
                    if (clientGroup != null && baseMessage != null)
                    {
                        clientGroup.HandleMessage(loggedClient, baseMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void HandleRBigInt(LoggedClient loggedClient, BaseMessage baseMessage)
        {
            try
            {
                var allGroups = clientsGroups;
                if (allGroups != null)
                {
                    var clientGroup = allGroups.Where(x => x.GroupContainsUser(loggedClient)).FirstOrDefault();
                    var rBigInt = RBigInt.Deserialize(baseMessage);
                    if (clientGroup != null && rBigInt != null)
                    {
                        clientGroup.HandleRBigIntRequest(loggedClient, rBigInt);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void HandleDHBigIntMessage(LoggedClient loggedClient, BaseMessage baseMessage)
        {
            try
            {
                var allGroups = clientsGroups;
                if (allGroups != null)
                {
                    var clientGroup = allGroups.Where(x => x.GroupContainsUser(loggedClient)).FirstOrDefault();
                    var dhBigInt = DHBigInt.Deserialize(baseMessage);
                    if (clientGroup != null && dhBigInt != null)
                    {
                        clientGroup.HandleDHBigIntRequest(loggedClient, dhBigInt);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void HandleInviteResponse(LoggedClient loggedClient, BaseMessage baseMessage)
        {
            try
            {
                InviteResponse inviteResponse = null;
                try
                {
                    inviteResponse = InviteResponse.Deserialize(baseMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                if (inviteResponse != null)
                {
                    var inviteGroup = GetClientInviteGroup(loggedClient);
                    if (inviteGroup != null)
                    {
                        inviteGroup.AddInviteResponse(loggedClient.Username, inviteResponse);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private ClientsGroupInvite GetClientInviteGroup(LoggedClient loggedClient)
        {
            for (int i = 0; i < clientsGroupInvites.Count; ++i)
            {
                var clients = clientsGroupInvites[i].LoggedClients;
                var client = clients.Where(x => x.Username == loggedClient.Username).FirstOrDefault();
                if (client != null)
                    return clientsGroupInvites[i];
            }
            return null;
        }

        private void InviteUsersToGroup(LoggedClient loggedClient, BaseMessage baseMessage)
        {
            try
            {
                StatusNotifier inviteMessage = null;
                try
                {
                    inviteMessage = StatusNotifier.Deserialize(baseMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                if (inviteMessage != null)
                {
                    if (AreUsersReadyToJoinGroup(inviteMessage))
                    {
                        CreateGroupAndInviteClients(loggedClient, inviteMessage);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void CreateGroupAndInviteClients(LoggedClient loggedClient, StatusNotifier inviteMessage)
        {
            lock(syncRootGroupsInvites)
            {
                var groupClientsList = new List<LoggedClient>() { loggedClient };
                var invitedUsernames = inviteMessage.Usernames;
                for (int i = 0; i < invitedUsernames.Count; ++i)
                {
                    var client = loggedClients.Where(x => x.Username == invitedUsernames[i]).FirstOrDefault();
                    if (client != null)
                    {
                        groupClientsList.Add(client);
                    }
                }
                if (groupClientsList.Count > 1)
                {
                    var group = new ClientsGroupInvite(groupClientsList);
                    group.GroupInviteTimeout += Group_GroupInviteTimeout;
                    group.GroupCreated += Group_GroupCreated;
                    clientsGroupInvites.Add(group);
                }
            }
        }

        private void Group_GroupCreated(object sender, GroupCreatedEventArgs e)
        {
            var groupClients = e.LoggedClients;
            var clientsResponse = e.InviteRosponses;
            var clientsGroupInvite = (ClientsGroupInvite)sender;
            try
            {
                clientsGroupInvites.Remove(clientsGroupInvite);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            var group = new ClientsGroup(groupClients, clientsResponse);
            lock(syncRootGroups)
            {
                if(clientsGroups != null)
                {
                    clientsGroups.Add(group);
                }
            }
        }

        private void Group_GroupInviteTimeout(object sender, GroupInviteTimeoutEventArgs e)
        {
            var group = (ClientsGroupInvite)sender;
            if (clientsGroupInvites != null)
            {
                lock (syncRootGroupsInvites)
                {
                    clientsGroupInvites.Remove(group);
                }
            }
        }

        private bool AreUsersReadyToJoinGroup(StatusNotifier inviteMessage)
        {
#warning sprawdz nie tylko liste zaproszen ale tez liste gotowych grup
            if(clientsGroupInvites != null)
            {
                lock(syncRootGroupsInvites)
                {
                    for(int i = 0; i < clientsGroupInvites.Count; i++)
                    {
                        var group = clientsGroupInvites[i];
                        var groupClientsUsernames = group.LoggedClients.Select(c => c.Username);
                        for(int j = 0; j < inviteMessage.Usernames.Count; ++j)
                        {
                            var username = inviteMessage.Usernames[j];
                            bool groupContainsUsername = groupClientsUsernames.Contains(username);
                            if (groupContainsUsername)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        private string GetLoginResponse(bool isAuthorized, string username, byte[] AESKey)
        {
            LoginResponse loginResponse = null;
            if (isAuthorized)
            {
                var users = loggedClients;
                loginResponse = new LoginResponse(true, users.Where(x => x.Username != username).Select(u => u.Username).ToList());
            }
            else
                loginResponse = new LoginResponse(false, new List<string>());
            var serializedLoginResponse = MessagesSerializer.Serialize(loginResponse);
            var baseMessage = new BaseMessage(MessageType.LoginResponse, serializedLoginResponse);
            var serializedResponse = MessagesSerializer.Serialize(baseMessage);
            return GetReadyToSendMessage(serializedResponse, AESKey);
        }

        private string GetReadyToSendMessage(string serializedBaseMessage, byte[] AESKey)
        {
            var AESIV = SecurityWrapper.GetRandomBytes();
            var encryptedMessage = EncryptMessage(serializedBaseMessage, AESKey, AESIV);
            return JoinIVToEncryptedMessage(encryptedMessage, AESIV);
        }

        private string JoinIVToEncryptedMessage(byte[] encryptedMessage, byte[] AESIV)
        {
            var message = SecurityWrapper.BytesToString(encryptedMessage);
            var iv = SecurityWrapper.BytesToString(AESIV);
            return iv + message;
        }

        private bool UsernameAndPasswordMatch(Login loginMessage)
        {
            var users = databaseCachedUsers;
            if (users != null)
            {
                var user = users.Where(u => u.user_name == loginMessage.Username).FirstOrDefault();
                if (user != null && passwordManager.AreEqual(loginMessage.Password, user.hash, user.salt))
                    return true;
            }
            return false;
        }

        private Login GetLoginMessage(StreamReader reader, byte[] AESKey)
        {
            string input = reader.ReadLine();
            var AESIV = GetIVFromInput(input);
            var encryptedMessage = GetEncryptedMessageFromInput(input);
            var decryptedMessage = AESWrapper.DecryptStringFromBytes(encryptedMessage, AESKey, AESIV);
            var baseMessage = BaseMessage.Deserialize(decryptedMessage);
            if (baseMessage.Type == MessageType.Login)
            {
                return Login.Deserialize(baseMessage);
            }
            else
                throw new InvalidOperationException();
        }

        private byte[] GetIVFromInput(string input)
        {
            if (input.Length >= 24)
            {
                string base64IV = input.Substring(0, 24);
                return SecurityWrapper.StringToBytes(base64IV);
            }
            else
                throw new InvalidOperationException();
        }

        private byte[] GetEncryptedMessageFromInput(string input)
        {
            if (input.Length >= 24)
            {
                string encryptedMessage = input.Substring(24);
                return SecurityWrapper.StringToBytes(encryptedMessage);
            }
            else
                throw new InvalidOperationException();
        }

        private byte[] EncryptMessage(string message, byte[] AESKey, byte[] AESIV)
        {
            return AESWrapper.EncryptStringToBytes(message, AESKey, AESIV);
        }

        private Register GetRegisterMessage(StreamReader reader)
        {
            string input = reader.ReadLine();
            var baseMessage = BaseMessage.Deserialize(input);
            if (baseMessage.Type == MessageType.Register)
            {
                return Register.Deserialize(baseMessage);
            }
            else
                throw new InvalidOperationException();
        }

        private string GetRegisterResponse(Register registerMessage, byte[] AESKey)
        {
            var response = AESWrapper.BytesToString(AESKey);
            var encryptedResponse = RSAWrapper.Encryption(response, registerMessage.Modulus, registerMessage.Exponent);
            return encryptedResponse;
        }
    }
}
