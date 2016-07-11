using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOM = Aras.IOM;

namespace Integrator.Connection.Aras
{
    public class ItemType : IItemType
    {

        internal Session Session { get; private set; }

        internal String ID { get; private set; }

        public IItemType Parent
        {
            get
            {
                return null;
            }
        }

        public Boolean CanVersion { get; private set; }

        public String Name { get; private set; }

        public Boolean SubType(IItemType Other)
        {
            if (this.Equals(Other))
            {
                return true;
            }
            else
            {
                if (this.Parent != null)
                {
                    return this.Parent.SubType(Other);
                }
                else
                {
                    return false;
                }
            }
        }

        private String[] _systemProperties;
        internal virtual String[] SystemProperties
        {
            get
            {
                if (this._systemProperties == null)
                {
                    this._systemProperties = new String[1] { "id" };
                }

                return this._systemProperties;
            }
        }

        private Dictionary<String, PropertyType> _propertyTypesCache;
        private Dictionary<String, PropertyType> PropertyTypesCache
        {
            get
            {
                if (this._propertyTypesCache == null)
                {
                    this._propertyTypesCache = new Dictionary<String, PropertyType>();

                    IOM.Item propertytypes = this.Session.Innovator.newItem("Property", "get");
                    propertytypes.setProperty("source_id", this.ID);
                    propertytypes.setAttribute("select", "name,data_type,stored_length,data_source");
                    propertytypes = propertytypes.apply();

                    if (!propertytypes.isError())
                    {
                        for (int i = 0; i < propertytypes.getItemCount(); i++)
                        {
                            IOM.Item propertype = propertytypes.getItemByIndex(i);
                            System.String propname = propertype.getProperty("name");

                            if (!this.SystemProperties.Contains(propname))
                            {
                                PropertyType proptype = null;

                                switch (propertype.getProperty("data_type"))
                                {
                                    case "string":
                                        proptype = new PropertyTypes.String(this, propname, System.Int32.Parse(propertype.getProperty("stored_length", "0")));
                                        break;
                                    case "item":
                                        proptype = new PropertyTypes.Item(this, propname, this.Session.ItemTypeByID(propertype.getProperty("data_source")));
                                        break;
                                    case "decimal":
                                        proptype = new PropertyTypes.Decimal(this, propname);
                                        break;
                                    case "list":
                                        proptype = new PropertyTypes.List(this, propname);
                                        break;
                                    case "date":
                                        proptype = new PropertyTypes.Date(this, propname);
                                        break;
                                    case "text":
                                        proptype = new PropertyTypes.Text(this, propname);
                                        break;
                                    case "integer":
                                        proptype = new PropertyTypes.Integer(this, propname);
                                        break;
                                    case "boolean":
                                        proptype = new PropertyTypes.Boolean(this, propname);
                                        break;
                                    case "image":
                                        proptype = new PropertyTypes.Image(this, propname);
                                        break;
                                    case "md5":
                                        proptype = new PropertyTypes.MD5(this, propname);
                                        break;
                                    case "float":
                                        proptype = new PropertyTypes.Float(this, propname);
                                        break;
                                    default:
                                        throw new NotImplementedException("PropertyType not implemented: " + propertype.getProperty("data_type"));
                                }

                                this._propertyTypesCache[proptype.Name] = proptype;
                            }
                        }
                    }
                    else
                    {
                        throw new Exceptions.ReadException(propertytypes.getErrorString());
                    }
                }

                return this._propertyTypesCache;
            }
        }

        public IEnumerable<IPropertyType> PropertyTypes
        {
            get
            {
                return this.PropertyTypesCache.Values;           
            }
        }

        public IPropertyType PropertyType(String Name)
        {
            if (this.PropertyTypesCache.ContainsKey(Name))
            {
                return this.PropertyTypesCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid PropertyType Name");
            }
        }

        private List<RelationshipType> _relationshipTypes;

        internal void AddRelationshipType(RelationshipType RelationshipType)
        {
            this._relationshipTypes.Add(RelationshipType);
        }

        public IEnumerable<IRelationshipType> RelationshipTypes
        {
            get
            {
                return this._relationshipTypes;
            }
        }

        public IRelationshipType RelationshipType(String Name)
        {
            foreach (IRelationshipType reltype in this.RelationshipTypes)
            {
                if (reltype.Name.Equals(Name))
                {
                    return reltype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid RelationshipType Name");
        }

        private String _tableName;
        internal String TableName
        {
            get
            {
                if (this._tableName == null)
                {
                    this._tableName = "[" + this.Name.ToLower().Replace(' ', '_') + "]";
                }

                return this._tableName;
            }
        }

        public bool Equals(IItemType other)
        {
            if (other != null)
            {
                if (other is ItemType)
                {
                    return this.Name.Equals(((ItemType)other).Name) && this.Session.Equals(((ItemType)other).Session);
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

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is IItemType)
                {
                    return this.Equals((IItemType)obj);
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
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Session Session, String ID, String Name, Boolean CanVersion)
        {
            this.Session = Session;
            this.ID = ID;
            this.Name = Name;
            this.CanVersion = CanVersion;
            this._relationshipTypes = new List<RelationshipType>();
        }
    }
}
