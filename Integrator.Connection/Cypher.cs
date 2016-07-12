using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Integrator.Connection
{
    public static class Cypher
    {
        private const int keysize = 256;

        private const String tokensep = "++++++++++++++++++++";

        private static string fixInitVector(string initVector)
        {
            return initVector.PadRight(16, '0').Substring(0, 16);
        }

        public static string Encrypt(Credentials Credentials, string Password, string Salt)
        {
            string plainText = Credentials.Group + tokensep + Credentials.Username + tokensep + Credentials.Password;
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(fixInitVector(Salt));
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Password, null);
            byte[] keyBytes = password.GetBytes(keysize/8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static Credentials Decrypt(string Token, string Password, string Salt)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(fixInitVector(Salt));
            byte[] cipherTextBytes = Convert.FromBase64String(Token);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Password, null);
            byte[] keyBytes = password.GetBytes(keysize/8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            String plain = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            int seppos1 = plain.IndexOf(tokensep, 0);
            int seppos2 = plain.IndexOf(tokensep, seppos1 + tokensep.Length);
            string grp = plain.Substring(0, seppos1);
            string user = plain.Substring(seppos1 + tokensep.Length, seppos2 - seppos1 - tokensep.Length);
            string pass = plain.Substring(seppos2 + tokensep.Length, plain.Length - seppos2 - tokensep.Length);
            return new Credentials(grp, user, pass);
        }
    }
}
