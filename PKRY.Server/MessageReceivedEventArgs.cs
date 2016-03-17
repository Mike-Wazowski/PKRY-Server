using PKRY.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Server
{
    public class MessageReceivedEventArgs:EventArgs
    {
        public BaseMessage Message
        {
            get;
            private set;
        }

        public MessageReceivedEventArgs(BaseMessage message)
        {
            Message = message;
        }
    }
}
