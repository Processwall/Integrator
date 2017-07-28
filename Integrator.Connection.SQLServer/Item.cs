using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.SQLServer
{
    public class Item : Connection.IItem
    {
        public Session Session { get; private set; }

        public Schema.ItemType ItemType { get; private set; }

        internal Table Table
        {
            get
            {
                return this.Session.Table(this.ItemType);
            }
        }

        public String ID { get; private set; }

        public String ConfigID { get; private set; }

        public Actions Action { get; internal set; }

        public IEnumerable<IItem> Versions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Dictionary<Schema.PropertyType, Property> PropertyCache;
        
        public IEnumerable<IProperty> Properties
        {
            get
            {
                return this.PropertyCache.Values;
            }
        }

        public IProperty Property(Schema.PropertyType PropertyType)
        {
            return this.PropertyCache[PropertyType];
        }

        public IProperty Property(String Name)
        {
            Schema.PropertyType proptype = this.ItemType.PropertyType(Name);
            return this.Property(proptype);
        }

        public IEnumerable<IRelationship> Relationships(Schema.RelationshipType RelationshipType)
        {
            String sql = "select " + this.Session.Table(RelationshipType).ColumnsSQL + " from " + this.Session.Table(RelationshipType).Name + " where sourceid='" + this.ID + "'";
            return this.Session.Table(RelationshipType).SelectRelationships(this, sql);
        }

        public IEnumerable<IRelationship> Relationships(String Name)
        {
            throw new NotImplementedException();
        }

        public IRelationship Create(Schema.RelationshipType RelationshipType)
        {
             String newid = this.Session.NewID();
             Relationship relationship = this.Session.GetRelationshipFromCache(RelationshipType, newid, newid, this, null);
             return relationship;
        }

        public IRelationship Create(String Name)
        {
            Schema.RelationshipType relationshiptype = this.ItemType.RelationshipType(Name);
            return this.Create(Name);
        }

        public IRelationship Create(Schema.RelationshipType RelationshipType, IItem Related)
        {
            throw new NotImplementedException();
        }

        public IRelationship Create(String Name, IItem Related)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void Lock()
        {
            this.Action = Actions.Update;
        }

        public void UnLock()
        {
            this.Action = Actions.Read;
        }

        public IItem Save()
        {
            switch(this.Action)
            {
                case Actions.Create:
                    this.Table.Insert(this);
                    this.Action = Actions.Update;
                    break;

                case Actions.Update:
                    this.Table.Update(this);
                    break;

                default:
                    throw new Integrator.Exceptions.ArgumentException("Action not supported:" + this.Action.ToString());
            }

            return this;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Boolean Equals(Connection.IItem other)
        {
            if (other != null)
            {
                return this.ID.Equals(other.ID);
            }
            else
            {
                return false;
            }
        }

        internal Item(Session Session, Schema.ItemType ItemType, String ID, String ConfigID)
        {
            this.Session = Session;
            this.ItemType = ItemType;
            this.ID = ID;
            this.ConfigID = ConfigID;
            this.PropertyCache = new Dictionary<Schema.PropertyType, Property>();

            foreach(Schema.PropertyType proptype in ItemType.PropertyTypes)
            {
                this.PropertyCache[proptype] = new Property(proptype);
            }
        }
    }
}
