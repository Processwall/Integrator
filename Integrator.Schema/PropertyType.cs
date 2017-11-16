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
    public abstract class PropertyType : IEquatable<PropertyType>
    {
        public ItemType ItemType { get; private set; }

        protected XmlNode Node { get; private set; }

        public String Name
        {
            get
            {
                if (this.Node.Attributes["name"] != null)
                {
                    return this.Node.Attributes["name"].Value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Property Type Name must be specified");
                }
            }
        }

        public Boolean FromConfig
        {
            get
            {
                if (this.ItemType.CanVersion)
                {
                    if (this.Node.Attributes["fromconfig"] != null)
                    {
                        if (String.Compare(this.Node.Attributes["fromconfig"].Value, "true", true) == 0)
                        {
                            return true;
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
                else
                {
                    return false;
                }
            }
        }

        public Boolean ReadOnly
        {
            get
            {
                if (this.Node.Attributes["readonly"] != null)
                {
                    if (String.Compare(this.Node.Attributes["readonly"].Value, "true", true) == 0)
                    {
                        return true;
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
        }

        public bool Equals(PropertyType other)
        {
            if (other != null)
            {
                return (this.Name.Equals(other.Name) && this.ItemType.Equals(other.ItemType));
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
                if (obj is PropertyType)
                {
                    return this.Equals((PropertyType)obj);
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
            return this.Name.GetHashCode() ^ this.ItemType.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal PropertyType(ItemType ItemType, XmlNode Node)
        {
            this.ItemType = ItemType;
            this.Node = Node;
        }
    }
}
