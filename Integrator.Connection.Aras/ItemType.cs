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

        public String Name { get; private set; }

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
                            PropertyType proptype = null;

                            switch (propertype.getProperty("data_type"))
                            {
                                case "string":
                                    proptype = new PropertyTypes.String(this, propertype.getProperty("name"), System.Int32.Parse(propertype.getProperty("stored_length", "0")));
                                    break;
                                case "item":
                                    proptype = new PropertyTypes.Item(this, propertype.getProperty("name"), this.Session.ItemTypeByID(propertype.getProperty("data_source")));
                                    break;
                                case "decimal":
                                    proptype = new PropertyTypes.Decimal(this, propertype.getProperty("name"));
                                    break;
                                case "list":
                                    proptype = new PropertyTypes.List(this, propertype.getProperty("name"));
                                    break;
                                case "date":
                                    proptype = new PropertyTypes.Date(this, propertype.getProperty("name"));
                                    break;
                                case "text":
                                    proptype = new PropertyTypes.Text(this, propertype.getProperty("name"));
                                    break;
                                case "integer":
                                    proptype = new PropertyTypes.Integer(this, propertype.getProperty("name"));
                                    break;
                                case "boolean":
                                    proptype = new PropertyTypes.Boolean(this, propertype.getProperty("name"));
                                    break;
                                case "image":
                                    proptype = new PropertyTypes.Image(this, propertype.getProperty("name"));
                                    break;
                                default:
                                    throw new NotImplementedException("PropertyType not implemented: " + propertype.getProperty("data_type"));
                            }

                            this._propertyTypesCache[proptype.Name] = proptype;
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

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Session Session, String ID, String Name)
        {
            this.Session = Session;
            this.ID = ID;
            this.Name = Name;
            this._relationshipTypes = new List<RelationshipType>();
        }
    }
}
