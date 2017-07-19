using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Integrator
{
    public class Parameters : System.Collections.Generic.IEnumerable<Parameter>
    {
        private const String parametersep = "++++++++++++++++++++";
        private const String valuesep = "--------------------";
        private const int keysize = 256;

        public Boolean ReadOnly { get; private set; }

        private static string fixInitVector(string initVector)
        {
            return initVector.PadRight(16, '0').Substring(0, 16);
        }

        private Dictionary<String, Integrator.Parameter> _parameters;

        public System.Collections.Generic.IEnumerator<Parameter> GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Integrator.Parameter Parameter(String Name)
        {
            if (this._parameters.ContainsKey(Name))
            {
                return this._parameters[Name];
            }
            else
            {
                throw new Integrator.Exceptions.ArgumentException("Invalid Parameter Name");
            }
        }

        public String Token (String Password, String Salt)
        {
            List<String> parameterstrings = new List<String>();

            foreach(Parameter parameter in this)
            {
                parameterstrings.Add(parameter.Name + valuesep + parameter.Value);
            }

            String plainText = String.Join(parametersep, parameterstrings);

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(fixInitVector(Salt));
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText.ToString());
            PasswordDeriveBytes password = new PasswordDeriveBytes(Password, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
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

        public Parameters(String[] Names)
        {
            this.ReadOnly = false;
            this._parameters = new Dictionary<String, Parameter>();

            foreach (String name in Names)
            {
                if (!this._parameters.ContainsKey(name))
                {
                    this._parameters[name] = new Parameter(this, name, null);
                }
                else
                {
                    throw new Integrator.Exceptions.ArgumentException("Duplicate Parameter Name");
                }
            }
        }

        public Parameters(String Token, String Password, String Salt)
        {
            this.ReadOnly = true;
            this._parameters = new Dictionary<String, Parameter>();

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(fixInitVector(Salt));
            byte[] cipherTextBytes = Convert.FromBase64String(Token);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Password, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            String plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            String[] parameterstrings = plainText.Split(new String[] { parametersep }, StringSplitOptions.None);

            foreach(String parameterstring in parameterstrings)
            {
                String[] parameterparts = parameterstring.Split(new String[] { valuesep }, StringSplitOptions.None);
                this._parameters[parameterparts[0]] = new Parameter(this, parameterparts[0], parameterparts[1]);
            }
        }
    }
}
