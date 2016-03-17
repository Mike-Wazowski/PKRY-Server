using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PKRY.Security
{
    public class RSAWrapper: SecurityWrapper
    {
        private const string _publicKeyFormat = "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>";

        public static string Encryption(string data, string modulus, string exponent)
        {
            try
            {
                CspParameters cspParams;
                cspParams = new CspParameters(1);
                cspParams.KeyContainerName = "Tracker";
                var RSA = new RSACryptoServiceProvider(cspParams);
                RSA.FromXmlString(GetPublicKey(modulus,exponent));
                byte[] byteData = Encoding.UTF8.GetBytes(data);
                byte[] cipherText = RSA.Encrypt(byteData, false);
                return Convert.ToBase64String(cipherText);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return String.Empty;
        }

        public static string Decryption(string data, string privateKeyXML)
        {
            try
            {
                var RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(privateKeyXML);
                byte[] ciphterText = Convert.FromBase64String(data);
                byte[] plainText = RSA.Decrypt(ciphterText, false);
                return Encoding.UTF8.GetString(plainText);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return String.Empty;
        }

        private static string GetPublicKey(string modulus, string exponent)
        {
            return String.Format(_publicKeyFormat, modulus, exponent);
        }
    }
}
