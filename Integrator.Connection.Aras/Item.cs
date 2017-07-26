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

        public Schema.ItemType ItemType { get; private set; }

        public String ID { get; private set; }

        public State Status { get; protected set; }

        private List<IItem> _versions;
        public IEnumerable<IItem> Versions
        {
            get
            {
                if (this._versions == null)
                {
                    if (this.ItemType.CanVersion)
                    {
                        String config_id = ((Property)this.Property("config_id")).DBValue;

                        IOM.Item iomrevisions = this.Session.Innovator.newItem(this.ItemType.Name, "get");
                        iomrevisions.setProperty("config_id", config_id);
                        iomrevisions.setProperty("generation", "0");
                        iomrevisions.setPropertyCondition("generation", "gt");
                        iomrevisions.setAttribute("orderBy", "generation");
                        iomrevisions.setAttribute("select", "id");
                        iomrevisions = iomrevisions.apply();

                        if (!iomrevisions.isError())
                        {
                            this._versions = new List<IItem>();

                            for (int i = 0; i < iomrevisions.getItemCount(); i++)
                            {
                                IOM.Item iomrevision = iomrevisions.getItemByIndex(i);
                                this._versions.Add(this.Session.Create(this.ItemType, iomrevision.getID(), State.Stored));
                            }
                        }
                        else
                        {
                            throw new Exceptions.ReadException(iomrevisions.getErrorString());
                        }
                    }
                    else
                    {
                        this._versions = new List<IItem>();
                        this._versions.Add(this);
                    }
                }

                return this._versions;
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

                            case "Float":
                                prop = new Properties.Float((PropertyTypes.Float)proptype);
                                break;

                            default:
                                throw new NotImplementedException("PropertyType not implemented: " + proptype.GetType().Name);
                        }

                        this._propertyCache[(PropertyType)proptype] = prop;
                    }

                    if ((this.Status == State.Stored) || (this.Status == State.Updating))
                    {
                        IOM.Item iomproperties = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "get");
                        iomproperties.setID(this.ID);
                        iomproperties = iomproperties.apply();

                        if (!iomproperties.isError())
                        {
                            foreach (Property property in this._propertyCache.Values)
                            {
                                property.DBValue = iomproperties.getProperty(property.PropertyType.Name);
                            }
                        }
                        else
                        {
                            throw new Exceptions.ReadException(iomproperties.getErrorString());
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

        public IProperty Property(Schema.PropertyType PropertyType)
        {
            if (this.PropertyCache.ContainsKey((PropertyType)PropertyType))
            {
                return this.PropertyCache[(PropertyType)PropertyType];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Property Name: " + PropertyType.Name);
            }
        }

        public IProperty Property(String Name)
        {
            return this.Property(this.ItemType.PropertyType(Name));
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
                    IOM.Item iomrels = this.Session.Innovator.newItem(((RelationshipType)RelationshipType).DBName, "get");
                    iomrels.setProperty("source_id", this.ID);
                    iomrels.setAttribute("select", "id,related_id,classification");
                    iomrels = iomrels.apply();

                    if (!iomrels.isError())
                    {
                        this.RelationshipsCache[(RelationshipType)RelationshipType] = new List<Relationship>();

                        for (int i = 0; i < iomrels.getItemCount(); i++)
                        {
                            IOM.Item iomrel = iomrels.getItemByIndex(i);
                            this.RelationshipsCache[(RelationshipType)RelationshipType].Add(this.Session.Create(((RelationshipType)RelationshipType).SubTypeFromClassification(iomrel.getProperty("classification")), iomrel.getID(), State.Stored, this.ID, iomrel.getProperty("related_id")));
                        }

                        return this.RelationshipsCache[(RelationshipType)RelationshipType];
                    }
                    else
                    {
                        String errormessage = iomrels.getErrorString();

                        if (errormessage.Equals("No items of type " + ((RelationshipType)RelationshipType).DBName + " found."))
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

        public virtual void Refresh()
        {
            this._versions = null;
            this._propertyCache = null;
            this.RelationshipsCache.Clear();
        }

        protected Int32 LockStatus()
        {
            IOM.Item locksttatus = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "get");
            locksttatus.setID(this.ID);
            return locksttatus.fetchLockStatus();
        }

        public void Lock()
        {
            switch (this.Status)
            {
                case State.Stored:
                case State.Updating:

                    switch(this.LockStatus())
                    {
                        case 0:

                            // Lock Item
                            IOM.Item lockitem = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "lock");
                            lockitem.setID(this.ID);
                            lockitem = lockitem.apply();

                            if (!lockitem.isError())
                            {
                                this.Status = State.Updating;
                            }
                            else
                            {
                                throw new Exceptions.UpdateException(lockitem.getErrorString());
                            }

                            break;
                        case 1:

                            // Already Locked by this User
                            this.Status = State.Updating;

                            break;
                        default:

                            // Locked by Another User
                            throw new Exceptions.UpdateException("Item Locked by another User");
                    }


                    break;
                default:
                    throw new Exceptions.ArgumentException("Item is not stored in Database");
            }
        }

        public void UnLock()
        {
            switch (this.Status)
            {
                case State.Stored:
                case State.Updating:

                    switch (this.LockStatus())
                    {
                        case 0:

                            // Already UnLocked
                            this.Status = State.Stored;

                            break;
                        case 1:

                            // UnLock Item
                            IOM.Item unlockitem = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "unlock");
                            unlockitem.setID(this.ID);
                            unlockitem = unlockitem.apply();

                            if (!unlockitem.isError())
                            {
                                this.Status = State.Stored;
                            }
                            else
                            {
                                throw new Exceptions.UpdateException(unlockitem.getErrorString());
                            }

                            break;
                        default:

                            // Locked by Another User
                            throw new Exceptions.UpdateException("Item Locked by another User");
                    }


                    break;
                default:
                    throw new Exceptions.ArgumentException("Item is not stored in Database");
            }
        }

        public virtual IItem Save(Boolean Unlock = true)
        {
            String action = null;

            switch(this.Status)
            {
                case State.Created:
                    action = "add";
                    break;
                case State.Updating:
                    action = "update";
                    break;

                case State.Stored:

                    throw new Exceptions.UpdateException("Item is not Locked");

                default:

                    throw new Exceptions.UpdateException("Item is Deleted");
            }

            IOM.Item iomitem = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, action);
            iomitem.setID(this.ID);
            iomitem.setProperty("classification", ((ItemType)this.ItemType).DBClassification);

            foreach(IPropertyType proptype in this.ItemType.PropertyTypes)
            {
                if (this.PropertyCache[(PropertyType)proptype].ValueSet)
                {
                    iomitem.setProperty(proptype.Name, this.PropertyCache[(PropertyType)proptype].DBValue);
                }
            }

            iomitem = iomitem.apply();

            if (!iomitem.isError())
            {
                if (iomitem.getID().Equals(this.ID))
                {
                    // Update Properties
                    foreach (Property property in this.PropertyCache.Values)
                    {
                        property.DBValue = iomitem.getProperty(property.PropertyType.Name);
                    }

                    this.Status = State.Updating;

                    if (Unlock)
                    {
                        // Unlock
                        this.UnLock();
                    }

                    return this;
                }
                else
                {
                    Item newitem = this.Session.Create((ItemType)this.ItemType, iomitem.getID(), State.Updating);

                    if (Unlock)
                    {
                        // Unlock
                        newitem.UnLock();
                    }

                    return newitem;
                }
            }
            else
            {
                throw new Exceptions.UpdateException(iomitem.getErrorString());
            }
        }

        public void Delete()
        {
            this.UnLock();

            IOM.Item deleteitem = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "delete");
            deleteitem.setID(this.ID);
            deleteitem = deleteitem.apply();

            if (!deleteitem.isError())
            {
                this.Status = State.Deleted;
            }
            else
            {
                throw new Exceptions.DeleteException(deleteitem.getErrorString());
            }
        }

        public IRelationship Create(IRelationshipType RelationshipType, IItem Related)
        {
            if ((RelationshipType != null) && (RelationshipType is RelationshipType) && RelationshipType.Source.Equals(this.ItemType))
            {
                String ID = this.Session.Innovator.getNewID();

                if (Related == null)
                {
                    return this.Session.Create((RelationshipType)RelationshipType, ID, State.Created, this.ID, null);
                }
                else
                {
                    return this.Session.Create((RelationshipType)RelationshipType, ID, State.Created, this.ID, Related.ID);
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid RelationshipType");
            }
        }

        public IRelationship Create(String Name, IItem Related)
        {
            return this.Create(this.ItemType.RelationshipType(Name), Related);
        }

        public IRelationship Create(IRelationshipType RelationshipType)
        {
            return this.Create(RelationshipType, null);
        }

        public IRelationship Create(String Name)
        {
            return this.Create(this.ItemType.RelationshipType(Name), null);
        }

        public bool Equals(IItem other)
        {
            if (other != null)
            {
                return this.ID.Equals(other.ID) && this.ItemType.Equals(other.ItemType);
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
                if (obj is IItem)
                {
                    return this.Equals((IItem)obj);
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
            return this.ID.GetHashCode() ^ this.ItemType.GetHashCode();
        }

        internal Item(Schema.ItemType ItemType, String ID, State Status)
        {
            this.ItemType = ItemType;
            this.ID = ID;
            this.Status = Status;
            this.RelationshipsCache = new Dictionary<Schema.RelationshipType, List<Relationship>>();
        }
    }
}
