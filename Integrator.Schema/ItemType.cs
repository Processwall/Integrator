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
        public Session DataModel { get; private set; }

        protected XmlNode Node { get; private set; }

        public ItemType Parent
        {
            get
            {
                XmlAttribute parentattribute = this.Node.Attributes["parent"];

                if (parentattribute != null)
                {
                    return this.DataModel.ItemType(parentattribute.Value);
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
                return this.Node.Attributes["name"].Value;
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

        public PropertyType PropertyType
        {
            get
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

            // Load Parent RelationshipTypes
            if (this.Parent != null)
            {
                foreach (RelationshipType reltype in this.Parent.RelationshipTypes)
                {
                    this.RelationshipTypesCache[reltype.Name] = reltype;
                }
            }

            // Load RelationshipTypes
            foreach (XmlNode reltypenode in this.Node.SelectNodes("relationshiptypes/relationshiptype"))
            {
                RelationshipType reltype = new RelationshipType(this, reltypenode);

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
            return this.Name.GetHashCode() ^ this.DataModel.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Session DataModel, XmlNode Node)
        {
            this.PropertyTypesCache = new Dictionary<String, PropertyType>();
            this.RelationshipTypesCache = new Dictionary<String, RelationshipType>();
            this.DataModel = DataModel;
            this.Node = Node;
            this.DataModel.AddItemTypeToCache(this);
            this.Load();
        }
    }
}
