using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOM = Aras.IOM;

namespace Integrator.Connection.Aras
{
    public class Item : Connection.IItem
    {
        internal Session Session
        {
            get
            {
                return ((ItemType)this.ItemType).Session;
            }
        }

        public Connection.IItemType ItemType { get; private set; }

        public String ID { get; private set; }

        internal Boolean InDatabase { get; private set; }

        public IEnumerable<IItem> Revisions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Dictionary<PropertyType, Property> _propertyCache;
        private Dictionary<PropertyType, Property> PropertyCache
        {
            get
            {
                if (this._propertyCache == null)
                {
                    this._propertyCache = new Dictionary<PropertyType, Property>();

                    foreach (Connection.IPropertyType proptype in this.ItemType.PropertyTypes)
                    {
                        Property prop = null;

                        switch (proptype.GetType().Name)
                        {
                            case "Boolean":
                                prop = new Properties.Boolean((PropertyTypes.Boolean)proptype);
                                break;

                            case "Date":
                                prop = new Properties.Date((PropertyTypes.Date)proptype);
                                break;

                            case "Decimal":
                                prop = new Properties.Decimal((PropertyTypes.Decimal)proptype);
                                break;

                            case "Image":
                                prop = new Properties.Image((PropertyTypes.Image)proptype);
                                break;

                            case "Integer":
                                prop = new Properties.Integer((PropertyTypes.Integer)proptype);
                                break;

                            case "Item":
                                prop = new Properties.Item((PropertyTypes.Item)proptype);
                                break;

                            case "List":
                                prop = new Properties.List((PropertyTypes.List)proptype);
                                break;

                            case "String":
                                prop = new Properties.String((PropertyTypes.String)proptype);
                                break;

                            case "Text":
                                prop = new Properties.Text((PropertyTypes.Text)proptype);
                                break;
                            case "MD5":
                                prop = new Properties.MD5((PropertyTypes.MD5)proptype);
                                break;

                            default:
                                throw new NotImplementedException("PropertyType not implemented: " + proptype.GetType().Name);
                        }

                        this._propertyCache[(PropertyType)proptype] = prop;
                    }

                    if (this.InDatabase)
                    {
                        IOM.Item iomproperties = this.Session.Innovator.newItem(this.ItemType.Name, "get");
                        iomproperties.setID(this.ID);
                        iomproperties = iomproperties.apply();

                        foreach (Property property in this._propertyCache.Values)
                        {
                            property.DBValue = iomproperties.getProperty(property.PropertyType.Name);
                        }
                    }
                }

                return this._propertyCache;
            }
        }

        public IEnumerable<IProperty> Properties
        {
            get
            {
                return this.PropertyCache.Values;
            }
        }

        private Dictionary<RelationshipType, List<Relationship>> RelationshipsCache;

        public IEnumerable<IRelationship> Relationships(IRelationshipType RelationshipType)
        {
            if ((RelationshipType != null) && (RelationshipType is RelationshipType) && RelationshipType.Source.Equals(this.ItemType))
            {
                if (this.RelationshipsCache.ContainsKey((RelationshipType)RelationshipType))
                {
                    return this.RelationshipsCache[(RelationshipType)RelationshipType];
                }
                else
                {
                    IOM.Item iomrels = this.Session.Innovator.newItem(RelationshipType.Name, "get");
                    iomrels.setProperty("source_id", this.ID);
                    iomrels.setAttribute("select", "id,related_id");
                    iomrels = iomrels.apply();

                    if (!iomrels.isError())
                    {
                        this.RelationshipsCache[(RelationshipType)RelationshipType] = new List<Relationship>();

                        for (int i = 0; i < iomrels.getItemCount(); i++)
                        {
                            IOM.Item iomrel = iomrels.getItemByIndex(i);
                            this.RelationshipsCache[(RelationshipType)RelationshipType].Add(this.Session.Create((RelationshipType)RelationshipType, iomrel.getID(), this.ID, iomrel.getProperty("related_id"), true));
                        }

                        return this.RelationshipsCache[(RelationshipType)RelationshipType];
                    }
                    else
                    {
                        String errormessage = iomrels.getErrorString();

                        if (errormessage.Equals("No items of type " + RelationshipType.Name + " found."))
                        {
                            return this.RelationshipsCache[(RelationshipType)RelationshipType] = new List<Relationship>();
                        }
                        else
                        {
                            throw new Exceptions.ReadException(errormessage);
                        }
                    }
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Relationship Type");
            }
        }

        public IEnumerable<IRelationship> Relationships(String Name)
        {
            return this.Relationships(this.ItemType.RelationshipType(Name));
        }

        internal Item(ItemType ItemType, String ID, Boolean InDatabase)
        {
            this.ItemType = ItemType;
            this.ID = ID;
            this.InDatabase = InDatabase;
            this.RelationshipsCache = new Dictionary<RelationshipType, List<Relationship>>();
        }
    }
}
