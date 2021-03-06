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
using System.Xml;

namespace Integrator.Schema
{
    public class ItemType : IEquatable<ItemType>
    {
        public Session Session { get; private set; }

        protected XmlNode Node { get; private set; }

        public ItemType Parent
        {
            get
            {
                XmlAttribute parentattribute = this.Node.Attributes["parent"];

                if (parentattribute != null)
                {
                    return this.Session.ItemType(parentattribute.Value);
                }
                else
                {
                    return null;
                }
            }
        }

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
                    throw new Exceptions.ArgumentException("Item Type Name must be specified");
                }
            }
        }

        public Boolean CanVersion
        {
            get
            {
                if (this.Parent != null)
                {
                    if (this.Parent.CanVersion)
                    {
                        return true;
                    }
                    else
                    {
                        if (this.Node.Attributes["canversion"] != null)
                        {
                            if (String.Compare(this.Node.Attributes["canversion"].Value, "true", true) == 0)
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
                else
                {
                    if (this.Node.Attributes["canversion"] != null)
                    {
                        if (String.Compare(this.Node.Attributes["canversion"].Value, "true", true) == 0)
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
        }

        private Dictionary<String, PropertyType> PropertyTypesCache;

        public IEnumerable<PropertyType> PropertyTypes
        {
            get
            {
                return this.PropertyTypesCache.Values;
            }
        }

        public Boolean HasProperty(String Name)
        {
            return this.PropertyTypesCache.ContainsKey(Name);
        }

        public PropertyType PropertyType(String Name)
        {
            if (this.HasProperty(Name))
            {
                return this.PropertyTypesCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Property Name: " + Name);
            }
        }

        public Boolean IsSubTypeOf(ItemType BaseItemType)
        {
            if (this.Parent != null)
            {
                if (this.Parent.Equals(BaseItemType))
                {
                    return true;
                }
                else
                {
                    return this.Parent.IsSubTypeOf(BaseItemType);
                }
            }
            else
            {
                return false;
            }
        }

        private Dictionary<String, RelationshipType> RelationshipTypesCache;

        public IEnumerable<RelationshipType> RelationshipTypes
        {
            get
            {
                return this.RelationshipTypesCache.Values;
            }
        }

        public RelationshipType RelationshipType(String Name)
        {
            if (this.RelationshipTypesCache.ContainsKey(Name))
            {
                return this.RelationshipTypesCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid RelationshipType Name: " + Name);
            }
        }

        private void Load()
        {
            // Load Parent PropertyTypes
            if (this.Parent != null)
            {
                foreach(PropertyType proptype in this.Parent.PropertyTypes)
                {
                    this.PropertyTypesCache[proptype.Name] = proptype;
                }
            }

            // Load PropertyTypes
            foreach (XmlNode proptypenode in this.Node.SelectNodes("propertytypes/propertytype"))
            {
                PropertyType proptype = null;

                switch (proptypenode.Attributes["type"].Value)
                {
                    case "String":
                        proptype = new PropertyTypes.String(this, proptypenode);
                        break;

                    case "Integer":
                        proptype = new PropertyTypes.Integer(this, proptypenode);
                        break;

                    case "Boolean":
                        proptype = new PropertyTypes.Boolean(this, proptypenode);
                        break;

                    case "Double":
                        proptype = new PropertyTypes.Double(this, proptypenode);
                        break;

                    case "List":
                        proptype = new PropertyTypes.List(this, proptypenode);
                        break;

                    case "Item":
                        proptype = new PropertyTypes.Item(this, proptypenode);
                        break;

                    case "Date":
                        proptype = new PropertyTypes.Date(this, proptypenode);
                        break;

                    case "Decimal":
                        proptype = new PropertyTypes.Decimal(this, proptypenode);
                        break;

                    case "Text":
                        proptype = new PropertyTypes.Text(this, proptypenode);
                        break;

                    default:

                        throw new Exceptions.ArgumentException("PropertyType not implemented: " + proptypenode.Attributes["type"].Value);
                }

                if (!this.PropertyTypesCache.ContainsKey(proptype.Name))
                {
                    this.PropertyTypesCache[proptype.Name] = proptype;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Duplicate PropertyType Name: " + this.Name + ": " + proptype.Name);
                }
            }

            // Load RelationshipTypes
            List<RelationshipType> reltypes = new List<RelationshipType>();

            foreach (XmlNode reltypenode in this.Node.SelectNodes("relationshiptypes/relationshiptype"))
            {
                reltypes.Add(new RelationshipType(this, reltypenode));
            }

            // Load Parent RelationshipTypes
            if (this.Parent != null)
            {
                foreach (RelationshipType parentreltype in this.Parent.RelationshipTypes)
                {
                    Boolean issubtype = false;

                    foreach(RelationshipType reltype in reltypes)
                    {
                        if (reltype.IsSubTypeOf(parentreltype))
                        {
                            issubtype = true;
                            break;
                        }
                    }

                    if (!issubtype)
                    {
                        this.RelationshipTypesCache[parentreltype.Name] = parentreltype;
                    }
                }
            }

            // Add RelationshipTypes to Cache
            foreach(RelationshipType reltype in reltypes)
            {
                if (!this.RelationshipTypesCache.ContainsKey(reltype.Name))
                {
                    this.RelationshipTypesCache[reltype.Name] = reltype;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Duplicate RelationshipType: " + reltype.Name);
                }
            }
        }


        public bool Equals(ItemType other)
        {
            if (other != null)
            {
                return (this.Name.Equals(other.Name) && this.Session.Equals(other.Session));
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
                if (obj is ItemType)
                {
                    return this.Equals((ItemType)obj);
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
            return this.Name.GetHashCode() ^ this.Session.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Session DataModel, XmlNode Node)
        {
            this.PropertyTypesCache = new Dictionary<String, PropertyType>();
            this.RelationshipTypesCache = new Dictionary<String, RelationshipType>();
            this.Session = DataModel;
            this.Node = Node;
            this.Session.AddItemTypeToCache(this);
            this.Load();
        }
    }
}
