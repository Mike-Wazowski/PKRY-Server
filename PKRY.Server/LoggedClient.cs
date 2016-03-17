using PKRY.Messages;
using PKRY.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PKRY.Server
{
    public class LoggedClient
    {
        public string Username
        {
            get;
            set;
        }

        private TcpClient TcpClient
        {
            get;
            set;
        }

        public byte[] AESKey
        {
            get;
            set;
        }

        public StreamReader Reader
        {
            get;
        }

        public StreamWriter Writer
        {
            get;
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public LoggedClient(string username, TcpClient tcpClient, byte[] AESKey)
        {
            Username = username;
            TcpClient = tcpClient;
            this.AESKey = AESKey;
            var stream = tcpClient.GetStream();
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
        }

        public void HandleIncomingRequests(string input)
        {
            if(!string.IsNullOrEmpty(input))
            {
                var handler = MessageReceived;
                if (handler != null)
                {
                    try
                    {
                        var message = GetMessageFromInput(input);
                        handler(this, new MessageReceivedEventArgs(message));
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private BaseMessage GetMessageFromInput(string input)
        {
            var AESIV = GetIVFromInput(input);
            var encryptedMessage = GetEncryptedMessageFromInput(input);
            var decryptedMessage = AESWrapper.DecryptStringFromBytes(encryptedMessage, AESKey, AESIV);
            var baseMessage = BaseMessage.Deserialize(decryptedMessage);
            return baseMessage;
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

        public void SendMessage(string serializedMessage)
        {
            var AESIV = SecurityWrapper.GetRandomBytes();
            var encryptedMessage = AESWrapper.EncryptStringToBytes(serializedMessage, AESKey, AESIV);
            var message = JoinIVToEncryptedMessage(encryptedMessage, AESIV);
            Writer.WriteLine(message);
            Writer.Flush();
        }

        private string JoinIVToEncryptedMessage(byte[] encryptedMessage, byte[] AESIV)
        {
            var message = SecurityWrapper.BytesToString(encryptedMessage);
            var iv = SecurityWrapper.BytesToString(AESIV);
            return iv + message;
        }
    }
}
