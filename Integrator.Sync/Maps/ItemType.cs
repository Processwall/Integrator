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
using System.Xml;

namespace Integrator.Sync.Maps
{
    public class ItemType
    {
        public Map Map { get; private set; }

        internal XmlNode Node { get; private set; }

        public Schema.ItemType Source
        {
            get
            {
                return this.Map.Source.Schema.ItemType(this.Node.Attributes["source"].Value);
            }
        }

        public Schema.ItemType Target
        {
            get
            {
                return this.Map.Target.Schema.ItemType(this.Node.Attributes["target"].Value);
            }
        }

        private List<Maps.PropertyType> _propertyTypeCache;
        private List<Maps.PropertyType> PropertyTypeCache
        {
            get
            {
                if (this._propertyTypeCache == null)
                {
                    this._propertyTypeCache = new List<Maps.PropertyType>();

                    foreach (XmlNode node in this.Node.SelectNodes("propertytypes/propertytype"))
                    {
                        Maps.PropertyType propertytype = new Sync.Maps.PropertyType(this, node);
                        this._propertyTypeCache.Add(propertytype);
                    }
                }

                return this._propertyTypeCache;
            }
        }

        public IEnumerable<Maps.PropertyType> PropertyTypes
        {
            get
            {
                return this.PropertyTypeCache;
            }
        }

        public Maps.PropertyType KeyPropertyType
        {
            get
            {
                PropertyType key = null;

                foreach(PropertyType proptype in this.PropertyTypes)
                {
                    if (proptype.Key)
                    {
                        key = proptype;
                        break;
                    }
                }

                if (key != null)
                {
                    return key;
                }
                else
                {
                    throw new Exceptions.MappingException("Key PropertyType not specified for: " + this.ToString());
                }
            }
        }

        public override string ToString()
        {
            return this.Source.Name + " -> " + this.Target.Name;
        }

        internal ItemType(Map Map, XmlNode Node)
        {
            this.Map = Map;
            this.Node = Node;
        }
    }
}
