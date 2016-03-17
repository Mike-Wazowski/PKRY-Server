using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Messages
{
    public static class MessagesSerializer
    {
        public static string Serialize(object messageToSerialize)
        {
            return JsonConvert.SerializeObject(messageToSerialize);
        }
    }
}
