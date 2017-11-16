/*  
  Integrator provides a set of .NET libraries for building migration and synchronisation 
  utilities for PLM (Product Lifecycle Management) Applications.

  Copyright (C) 2017 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Integrator.Connection
{
    public class Parameters : System.Collections.Generic.IEnumerable<Parameter>
    {
        private const String parametersep = "++++++++++++++++++++";
        private const String valuesep = "--------------------";

        public String ApplicationID { get; private set; }

        public DataProtectionScope Scope { get; private set; }

        public Boolean ReadOnly { get; private set; }

        private Dictionary<String, Integrator.Connection.Parameter> _parameters;

        public System.Collections.Generic.IEnumerator<Parameter> GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Integrator.Connection.Parameter Parameter(String Name)
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

        public Boolean HasParamter(String Name)
        {
            return this._parameters.ContainsKey(Name);
        }

        public String Token ()
        {
            List<String> parameterstrings = new List<String>();

            foreach(Parameter parameter in this)
            {
                parameterstrings.Add(parameter.Name + valuesep + parameter.Value);
            }

            String plainText = String.Join(parametersep, parameterstrings);
            
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] entropyBytes = Encoding.Unicode.GetBytes(this.ApplicationID);
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, entropyBytes, this.Scope);

            return Convert.ToBase64String(encryptedBytes);
        }

        public Parameters(String ApplicationID, DataProtectionScope Scope, IEnumerable<String> Names)
        {
            this.ApplicationID = ApplicationID;
            this.Scope = Scope;
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

        public Parameters(String ApplicationID, DataProtectionScope Scope, String Token)
        {
            this.ApplicationID = ApplicationID;
            this.Scope = Scope;
            this.ReadOnly = true;
            this._parameters = new Dictionary<String, Parameter>();

            byte[] encryptedData = Convert.FromBase64String(Token);
            byte[] entropyBytes = Encoding.Unicode.GetBytes(this.ApplicationID);
            byte[] decryptedData = ProtectedData.Unprotect(encryptedData, entropyBytes, this.Scope);
            String plainText = Encoding.Unicode.GetString(decryptedData, 0, decryptedData.Length);

            String[] parameterstrings = plainText.Split(new String[] { parametersep }, StringSplitOptions.None);

            foreach(String parameterstring in parameterstrings)
            {
                String[] parameterparts = parameterstring.Split(new String[] { valuesep }, StringSplitOptions.None);
                this._parameters[parameterparts[0]] = new Parameter(this, parameterparts[0], parameterparts[1]);
            }
        }
    }
}
