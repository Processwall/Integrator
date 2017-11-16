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

namespace Integrator.Sync
{
    public class Map
    {
        public Session Session { get; private set; }

        public Integrator.Log Log
        {
            get
            {
                return this.Session.Log;
            }
        }

        internal XmlNode Node { get; private set; }

        public String Name
        {
            get
            {
                return this.Node.Attributes["name"].Value;
            }
        }

        public Connection.Session Source
        {
            get
            {
                return this.Session.Connection(this.Node.Attributes["source"].Value);
            }
        }

        public Connection.Session Target
        {
            get
            {
                return this.Session.Connection(this.Node.Attributes["target"].Value);
            }
        }

        private List<Maps.ItemType> _itemTypeCache;
        private List<Maps.ItemType> ItemTypeCache
        {
            get
            {
                if (this._itemTypeCache == null)
                {
                    this._itemTypeCache = new List<Maps.ItemType>();

                    foreach (XmlNode node in this.Node.SelectNodes("itemtypes/itemtype"))
                    {
                        Maps.ItemType itemtype = new Sync.Maps.ItemType(this, node);
                        this._itemTypeCache.Add(itemtype);
                    }
                }

                return this._itemTypeCache;
            }
        }

        public IEnumerable<Maps.ItemType> ItemTypes
        {
            get
            {
                return this.ItemTypeCache;
            }
        }

        public Maps.ItemType ItemTypeBySource(String Name)
        {
            foreach(Maps.ItemType itemtype in this.ItemTypes)
            {
                if (itemtype.Source.Name.Equals(Name))
                {
                    return itemtype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid Source ItemType Name");
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal Map(Session Session, XmlNode Node)
        {
            this.Session = Session;
            this.Node = Node;
        }
    }
}
