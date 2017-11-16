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
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Integrator.Connection.Token
{
    public class Session
    {
        public String Name { get; private set; }

        public FileInfo Filename { get; private set; }

        private Assembly _assembly;
        private Assembly Assembly
        {
            get
            {
                if (this._assembly == null)
                {
                    this._assembly = Assembly.LoadFrom(this.Filename.FullName);
                }

                return this._assembly;
            }
        }

        private List<Type> _connectionTypes;
        public IEnumerable<Type> ConnectionTypes
        {
            get
            {
                if (this._connectionTypes == null)
                {
                    this._connectionTypes = new List<Type>();

                    foreach(Type type in this.Assembly.GetTypes())
                    {
                        if ((type.BaseType != null) && (type.BaseType == typeof(Connection.Session)))
                        {
                            this._connectionTypes.Add(type);
                        }
                    }
                }

                return this._connectionTypes;
            }
        }

        public Session(String Name, FileInfo Filename)
        {
            this.Name = Name;
            this.Filename = Filename;
        }
    }
}
