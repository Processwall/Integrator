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

namespace Integrator.Connection.SQLServer
{
    internal class Column
    {
        internal Table Table { get; private set; }

        internal String Name { get; private set; }

        internal String Type { get; private set; }

        internal Boolean IsNullable { get; private set; }

        internal Int32 MaxLength { get; private set; }

        internal Boolean PrimaryIndex { get; private set; }

        internal Boolean Exists { get; private set; }

        private String _sQL;
        internal String SQL
        {
            get
            {
                if (this._sQL == null)
                {
                    this._sQL = "[" + this.Name + "] " + this.Type;

                    if (this.Type == "nvarchar")
                    {
                        this._sQL += "(" + this.MaxLength + ")";
                    }
                    else if (this.Type == "decimal")
                    {
                        this._sQL += "(18,5)";
                    }

                    if (!this.IsNullable)
                    {
                        this._sQL += " not null";
                    }

                    if (this.PrimaryIndex)
                    {
                        this._sQL += " primary key";
                    }
                }

                return this._sQL;
            }
        }

        public override string ToString()
        {
            return this.Name + " (" + this.Type + ")";
        }

        internal Column(Table Table, String Name, String Type, Boolean IsNullable, Int32 MaxLength, Boolean PrimaryIndex, Boolean Exists)
        {
            this.Table = Table;
            this.Name = Name;
            this.Type = Type;
            this.IsNullable = IsNullable;
            this.MaxLength = MaxLength;
            this.PrimaryIndex = PrimaryIndex;
            this.Exists = Exists;
        }
    }
}
