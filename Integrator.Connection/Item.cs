﻿/*  
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

namespace Integrator.Connection
{
    public class Item
    {
        public enum States { Create, Read, Update, Delete };

        public Session Session { get; private set; }

        public Schema.ItemType ItemType { get; private set; }

        public States State { get; private set; }

        public String ID { get; set; }

        public String ConfigID { get; set; }

        internal void Created(String ID, String ConfigID)
        {
            switch (this.State)
            {
                case States.Create:
                    this.ID = ID;
                    this.ConfigID = ConfigID;
                    this.State = States.Read;
                    break;
                default:
                    break;
            }
        }

        internal void Updated()
        {
            switch (this.State)
            {
                case States.Update:
                    this.State = States.Read;
                    break;
                default:
                    break;
            }
        }

        internal void Update()
        {
            switch (this.State)
            {
                case States.Read:
                case States.Update:
                    this.State = States.Update;
                    break;
                default:
                    break;
            }
        }

        internal void Delete()
        {
            switch (this.State)
            {
                case States.Read:
                case States.Update:
                    this.State = States.Delete;
                    break;
                default:
                    break;
            }
        }

        private Dictionary<Schema.PropertyType, Object> PropertyCache;

        public void SetProperty(Schema.PropertyType PropertyType, Object Value)
        {
            if (PropertyType != null)
            {
                this.PropertyCache[PropertyType] = Value;
            }
            else
            {
                throw new Exceptions.ArgumentException("PropertyType must be specified");
            }
        }

        public void SetProperty(String Name, Object Value)
        {
            Schema.PropertyType proptype = this.ItemType.PropertyType(Name);
            this.SetProperty(proptype, Value);
        }

        public Object GetProperty(Schema.PropertyType PropertyType)
        {
            if (PropertyType != null)
            {
                if (!this.PropertyCache.ContainsKey(PropertyType))
                {
                    this.PropertyCache[PropertyType] = null;
                }

                return this.PropertyCache[PropertyType];

            }
            else
            {
                throw new Exceptions.ArgumentException("PropertyType must be specified");
            }
        }

        public Object GetProperty(String Name)
        {
            Schema.PropertyType proptype = this.ItemType.PropertyType(Name);
            return this.GetProperty(proptype);
        }

        public void CopyProperties(Item Item)
        {
            if (this.ItemType.Equals(Item.ItemType))
            {
                foreach(Schema.PropertyType proptype in this.ItemType.PropertyTypes)
                {
                    this.SetProperty(proptype, Item.GetProperty(proptype));
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("ItemTypes do not match");
            }
        }

        internal Item(Session Session, Schema.ItemType ItemType, States State, String ID, String ConfigID)
        {
            this.Session = Session;
            this.ItemType = ItemType;
            this.ID = ID;
            this.ConfigID = ConfigID;
            this.State = State;
            this.PropertyCache = new Dictionary<Schema.PropertyType, Object>();
        }
    }
}
