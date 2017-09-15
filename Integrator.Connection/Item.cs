using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Item : IEquatable<Item>
    {
        public enum Actions { Create, Read, Update, Delete };

        public Session Session { get; private set; }

        public Schema.ItemType ItemType { get; private set; }

        public Actions Action { get; private set; }

        private Guid SessionID;

        public String ID { get; private set; }

        public String ConfigID { get; private set; }

        public abstract IEnumerable<Item> Versions { get; }

        private Dictionary<Schema.PropertyType, Property> PropertyCache;

        public IEnumerable<Property> Properties
        {
            get
            {
                return this.PropertyCache.Values;
            }
        }

        public Property Property(Schema.PropertyType PropertyType)
        {
            if (this.PropertyCache.ContainsKey(PropertyType))
            {
                return this.PropertyCache[PropertyType];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid PropertyType");
            }
        }

        public Property Property(String Name)
        {
            Schema.PropertyType proptype = this.ItemType.PropertyType(Name);
            return this.Property(proptype);
        }

        public abstract IEnumerable<Relationship> Relationships(Schema.RelationshipType RelationshipType);

        public IEnumerable<Relationship> Relationships(String RelationshipTypeName)
        {
            Schema.RelationshipType relationshiptype = this.ItemType.RelationshipType(RelationshipTypeName);
            return this.Relationships(relationshiptype);
        }

        public Relationship Create(Transaction Transaction, Schema.RelationshipType RelationshipType)
        {
            return this.Create(Transaction, RelationshipType, null);
        }

        public Relationship Create(Transaction Transaction, String RelationshipTypeName)
        {
            Schema.RelationshipType relationshiptype = this.ItemType.RelationshipType(RelationshipTypeName);
            return this.Create(Transaction, relationshiptype);
        }

        public Relationship Create(Transaction Transaction, Schema.RelationshipType RelationshipType, Item Related)
        {
            throw new NotImplementedException();
        }

        public Relationship Create(Transaction Transaction, String RelationshipTypeName, Item Related)
        {
            Schema.RelationshipType relationshiptype = this.ItemType.RelationshipType(RelationshipTypeName);
            return this.Create(Transaction, relationshiptype, Related);
        }

        public void Refresh()
        {

        }

        public void Update(Transaction Transaction)
        {
            this.Action = Actions.Update;
            Transaction.Add(this);
        }

        public void Delete(Transaction Transaction)
        {
            this.Action = Actions.Delete;
            Transaction.Add(this);
        }

        internal void ItemCreated(String ID, String ConfigID)
        {
            if (this.Action == Actions.Create)
            {
                this.ID = ID;
                this.ConfigID = ConfigID;
                this.Action = Actions.Read;
                this.Session.AddToCache(this);
            }
            else
            {
                throw new Exceptions.ArgumentException("Item already created");
            }
        }

        public Boolean Equals(Item other)
        {
            if (other != null)
            {
                if (this.Action == Actions.Create)
                {
                    return this.SessionID.Equals(other.SessionID);
                }
                else
                {
                    return this.ID.Equals(other.ID);
                }
            }
            else
            {
                return false;
            }
        }

        public override Boolean Equals(Object obj)
        {
            if ((obj != null) && (obj is Item))
            {
                return this.Equals((Item)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (this.Action == Actions.Create)
            {
                return this.SessionID.GetHashCode();
            }
            else
            {
                return this.ID.GetHashCode();
            }
        }

        private void Initialise(Session Session, Schema.ItemType ItemType)
        {
            this.Session = Session;
            this.ItemType = ItemType;
            this.SessionID = Guid.NewGuid();
            this.PropertyCache = new Dictionary<Schema.PropertyType, Property>();

            foreach(Schema.PropertyType proptype in this.ItemType.PropertyTypes)
            {
                switch (proptype.GetType().Name)
                {
                    case "Boolean":
                        this.PropertyCache[proptype] = new Properties.Boolean(this, (Integrator.Schema.PropertyTypes.Boolean)proptype);
                        break;
                    case "Date":
                        this.PropertyCache[proptype] = new Properties.Date(this, (Integrator.Schema.PropertyTypes.Date)proptype);
                        break;
                    case "Decimal":
                        this.PropertyCache[proptype] = new Properties.Decimal(this, (Integrator.Schema.PropertyTypes.Decimal)proptype);
                        break;
                    case "Double":
                        this.PropertyCache[proptype] = new Properties.Double(this, (Integrator.Schema.PropertyTypes.Double)proptype);
                        break;
                    case "Integer":
                        this.PropertyCache[proptype] = new Properties.Integer(this, (Integrator.Schema.PropertyTypes.Integer)proptype);
                        break;
                    case "Item":
                        this.PropertyCache[proptype] = new Properties.Item(this, (Integrator.Schema.PropertyTypes.Item)proptype);
                        break;
                    case "List":
                        this.PropertyCache[proptype] = new Properties.List(this, (Integrator.Schema.PropertyTypes.List)proptype);
                        break;
                    case "String":
                        this.PropertyCache[proptype] = new Properties.String(this, (Integrator.Schema.PropertyTypes.String)proptype);
                        break;
                    case "Text":
                        this.PropertyCache[proptype] = new Properties.Text(this, (Integrator.Schema.PropertyTypes.Text)proptype);
                        break;
                    default:
                        throw new Integrator.Exceptions.ArgumentException("Property Type not implemented: " + proptype.GetType().Name);
                }
            }
        }

        public Item(Session Session, Schema.ItemType ItemType)
        {
            this.Initialise(Session, ItemType);
            this.Action = Actions.Create;
            this.ID = null;
            this.ConfigID = null;
        }

        public Item(Session Session, Schema.ItemType ItemType, String ID, String ConfigID)
        {
            this.Initialise(Session, ItemType);
            this.Action = Actions.Read;
            this.ID = ID;
            this.ConfigID = ConfigID;
            this.Session.AddToCache(this);
        }
    }
}
