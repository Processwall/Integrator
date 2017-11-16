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
using System.Xml;

namespace Integrator.Schema
{
    public class List : IEquatable<List>
    {
        public Session DataModel { get; private set; }

        private XmlNode Node { get; set; }

        public String Name
        {
            get
            {
                return this.Node.Attributes["name"].Value;
            }
        }

        private Dictionary<String, ListValue> ListValuesCache;

        public IEnumerable<ListValue> Values
        {
            get
            {
                return this.ListValuesCache.Values;
            }
        }

        public ListValue Value(String Value)
        {
            if (Value != null)
            {
                foreach (ListValue listvalue in this.Values)
                {
                    if (listvalue.Value.Equals(Value))
                    {
                        return listvalue;
                    }
                }

                throw new Exceptions.ArgumentException("Invalid List Value");
            }
            else
            {
                return null;
            }
        }

        private void Load()
        {
            // Load Values
            foreach (XmlNode valuenode in this.Node.SelectNodes("values/value"))
            {
                ListValue listvalue = new ListValue(this, valuenode);

                if (!this.ListValuesCache.ContainsKey(listvalue.Value))
                {
                    this.ListValuesCache[listvalue.Value] = listvalue;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Duplicate List Value: " + this.Name + ": " + listvalue.Value);
                }
            }
        }

        public bool Equals(List other)
        {
            if (other != null)
            {
                return (this.Name.Equals(other.Name) && this.DataModel.Equals(other.DataModel));
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is List)
                {
                    return this.Equals((List)obj);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.DataModel.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal List(Session DataModel, XmlNode Node)
        {
            this.ListValuesCache = new Dictionary<String, ListValue>();
            this.DataModel = DataModel;
            this.Node = Node;
            this.Load();
        }
    }
}
