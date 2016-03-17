using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Security
{
    public class SecurityWrapper
    {
        public static byte[] GetRandomBytes()
        {
            var guid = Guid.NewGuid().ToByteArray();
            return guid;
        }

        public static string BytesToString(byte[] bytesToConvert)
        {
            return Convert.ToBase64String(bytesToConvert);
        }

        public static byte[] StringToBytes(string stringToConvert)
        {
            return Convert.FromBase64String(stringToConvert);
        }
    }
}
