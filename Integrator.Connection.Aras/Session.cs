using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOM = Aras.IOM;

namespace Integrator.Connection.Aras
{
    public class Session : ISession
    {
        public String URL { get; private set; }

        public String Database { get; private set; }

        public String Username { get; private set; }

        public String Password { get; private set; }

        public bool Equals(ISession other)
        {
            if (other != null)
            {
                if (other is Session)
                {
                    return this.URL.Equals(((Session)other).URL) && this.Database.Equals(((Session)other).Database) && this.Username.Equals(((Session)other).Username);
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

        private IOM.Innovator _innovator;
        internal IOM.Innovator Innovator
        {
            get
            {
                if (this._innovator == null)
                {
                    IOM.HttpServerConnection connection = IOM.IomFactory.CreateHttpServerConnection(this.URL, this.Database, this.Username, this.Password);
                    IOM.Item user = connection.Login();

                    if (!user.isError())
                    {
                        this._innovator = IOM.IomFactory.CreateInnovator(connection);
                    }
                    else
                    {
                        throw new Exceptions.LoginException(user.getErrorString());
                    }
                }

                return this._innovator;
            }
        }

        private Dictionary<String, ItemType> _itemTypeCache;
        private Dictionary<String, ItemType> ItemTypeCache
        {
            get
            {
                if (this._itemTypeCache == null)
                {
                    this._itemTypeCache = new Dictionary<String, ItemType>();

                    // Read ItemTypes
                    IOM.Item iomitemtypes = this.Innovator.newItem("ItemType", "get");
                    iomitemtypes.setAttribute("select", "id,name,is_relationship,is_versionable");
                    iomitemtypes = iomitemtypes.apply();

                    if (!iomitemtypes.isError())
                    {
                        for (int i = 0; i < iomitemtypes.getItemCount(); i++)
                        {
                            IOM.Item iomitemtype = iomitemtypes.getItemByIndex(i);

                            if (iomitemtype.getProperty("is_relationship", "0").Equals("1"))
                            {
                                RelationshipType relationshiptype = new RelationshipType(this, iomitemtype.getID(), iomitemtype.getProperty("name"), iomitemtype.getProperty("is_versionable", "0").Equals("1"));
                                this.ItemTypeCache[relationshiptype.ID] = relationshiptype;
                            }
                            else
                            {
                                ItemType itemtype = new ItemType(this, iomitemtype.getID(), iomitemtype.getProperty("name"), iomitemtype.getProperty("is_versionable", "0").Equals("1"));
                                this.ItemTypeCache[itemtype.ID] = itemtype;
                            }
                        }
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomitemtypes.getErrorString());
                    }

                    // Read RelationshipTypes
                    IOM.Item iomrelationshiptypes = this.Innovator.newItem("RelationshipType", "get");
                    iomrelationshiptypes.setAttribute("select", "source_id,related_id,relationship_id");
                    iomrelationshiptypes = iomrelationshiptypes.apply();

                    if (!iomrelationshiptypes.isError())
                    {
                        for (int i = 0; i < iomrelationshiptypes.getItemCount(); i++)
                        {
                            IOM.Item iomrelationshiptype = iomrelationshiptypes.getItemByIndex(i);
                            
                            // Store Source
                            String source_id = iomrelationshiptype.getProperty("source_id", null);

                            if (!String.IsNullOrEmpty(source_id))
                            {
                                ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Source = this._itemTypeCache[source_id];
                            }

                            // Store Related
                            String related_id = iomrelationshiptype.getProperty("related_id", null);

                            if (!String.IsNullOrEmpty(related_id))
                            {
                                ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Related = this._itemTypeCache[related_id];
                            }

                        }
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomitemtypes.getErrorString());
                    }
                }

                return this._itemTypeCache;
            }
        }

        private List<ItemType> _itemTypes;
        public IEnumerable<IItemType> ItemTypes
        {
            get
            {
                if (this._itemTypes == null)
                {
                    this._itemTypes = new List<ItemType>();

                    foreach (ItemType itemtype in this.ItemTypeCache.Values)
                    {
                        if (!(itemtype is RelationshipType))
                        {
                            this._itemTypes.Add(itemtype);
                        }
                    }
                }

                return this._itemTypes;
            }
        }

        internal ItemType ItemTypeByID(String ID)
        {
            if (this.ItemTypeCache.ContainsKey(ID))
            {
                return this.ItemTypeCache[ID];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType ID: " + ID);
            }
        }

        public IItemType ItemType(String Name)
        {
            foreach (ItemType itemtype in this.ItemTypeCache.Values)
            {
                if (itemtype.Name.Equals(Name))
                {
                    return itemtype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid ItemType Name: " + Name);
        }

        private Dictionary<ItemType, Dictionary<String, Item>> ItemCache;

        internal Item Create(ItemType ItemType, String ID, State Status)
        {
            if (!this.ItemCache.ContainsKey(ItemType))
            {
                this.ItemCache[ItemType] = new Dictionary<String,Item>();
            }

            if (this.ItemCache[ItemType].ContainsKey(ID))
            {
                return this.ItemCache[ItemType][ID];
            }
            else
            {
                if (ItemType is RelationshipType)
                {
                    IOM.Item iomsource = this.Innovator.newItem(ItemType.Name, "get");
                    iomsource.setID(ID);
                    iomsource.setAttribute("select", "id,source_id,related_id");
                    iomsource = iomsource.apply();

                    if (!iomsource.isError())
                    {
                        return this.Create((RelationshipType)ItemType, ID, State.Stored, iomsource.getProperty("source_id"), iomsource.getProperty("related_id"));
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomsource.getErrorString());
                    }
                }
                else
                {
                    Item item = new Item(ItemType, ID, Status);
                    this.ItemCache[ItemType][ID] = item;
                    return item;
                }
            }
        }

        internal Relationship Create(RelationshipType RelationshipType, String ID, State Status, String SourceID, String RelatedID)
        {
            if (!this.ItemCache.ContainsKey(RelationshipType))
            {
                this.ItemCache[RelationshipType] = new Dictionary<String, Item>();
            }

            if (this.ItemCache[RelationshipType].ContainsKey(ID))
            {
                return (Relationship)this.ItemCache[RelationshipType][ID];
            }
            else
            {
                Item source = null;
                Item related = null;

                if (!System.String.IsNullOrEmpty(SourceID))
                {
                    if (RelationshipType.Source is RelationshipType)
                    {
                        IOM.Item iomsource = this.Innovator.newItem(RelationshipType.Source.Name, "get");
                        iomsource.setID(SourceID);
                        iomsource.setAttribute("select", "id,source_id,related_id");
                        iomsource = iomsource.apply();

                        if (!iomsource.isError())
                        {
                            source = this.Create((RelationshipType)RelationshipType.Source, SourceID, State.Stored, iomsource.getProperty("source_id"), iomsource.getProperty("related_id"));
                        }
                        else
                        {
                            throw new Exceptions.ReadException(iomsource.getErrorString());
                        }
                    }
                    else
                    {
                        source = this.Create((ItemType)RelationshipType.Source, SourceID, Status);
                    }
                }

                if (RelationshipType.Related != null)
                {
                    if (!System.String.IsNullOrEmpty(RelatedID))
                    {
                        if (RelationshipType.Related is RelationshipType)
                        {
                            IOM.Item iomrelated = this.Innovator.newItem(RelationshipType.Related.Name, "get");
                            iomrelated.setID(RelatedID);
                            iomrelated.setAttribute("select", "id,source_id,related_id");
                            iomrelated = iomrelated.apply();

                            if (!iomrelated.isError())
                            {
                                related = this.Create((RelationshipType)RelationshipType.Related, RelatedID, State.Stored, iomrelated.getProperty("source_id"), iomrelated.getProperty("related_id"));
                            }
                            else
                            {
                                throw new Exceptions.ReadException(iomrelated.getErrorString());
                            }
                        }
                        else
                        {
                            related = this.Create((ItemType)RelationshipType.Related, RelatedID, State.Stored);
                        }
                    }
                }

                Relationship relationship = new Relationship(RelationshipType, ID, Status, source, related);
                this.ItemCache[RelationshipType][ID] = relationship;
                return relationship;
            }
        }

        public IItem Create(IItemType ItemType)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                String ID = this.Innovator.getNewID();
                Item item = this.Create((ItemType)ItemType, ID, State.Created);
                return item;
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public IItem Create(String Name)
        {
            return this.Create(this.ItemType(Name));
        }

        public IEnumerable<IItem> Index(IItemType ItemType)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                IOM.Item iomitems = this.Innovator.newItem(ItemType.Name, "get");
                iomitems.setAttribute("select", "id");
                iomitems = iomitems.apply();

                if (!iomitems.isError())
                {
                    List<Item> items = new List<Item>();

                    for (int i = 0; i < iomitems.getItemCount(); i++)
                    {
                        IOM.Item iomitem = iomitems.getItemByIndex(i);
                        items.Add(this.Create((ItemType)ItemType, iomitem.getID(), State.Stored));
                    }

                    return items;
                }
                else
                {
                    String errormessage = iomitems.getErrorString();

                    if (errormessage.Equals("No items of type " + ItemType.Name + " found."))
                    {
                        return new List<Item>();
                    }
                    else
                    {
                        throw new Exceptions.ReadException(errormessage);
                    }
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public IEnumerable<IItem> Index(String Name)
        {
            return this.Index(this.ItemType(Name));
        }

        private static System.String OperatorString(Conditions.Operators Operator)
        {
            switch (Operator)
            {
                case Conditions.Operators.eq:
                    return "=";
                case Conditions.Operators.ne:
                    return "<>";
                case Conditions.Operators.gt:
                    return ">";
                case Conditions.Operators.lt:
                    return "<";
                case Conditions.Operators.le:
                    return "<=";
                case Conditions.Operators.ge:
                    return ">=";
                case Conditions.Operators.like:
                    return "like";
                default:
                    throw new NotImplementedException("Property Condition Operator not implemented");
            }
        }

        internal String Where(IItemType ItemType, Condition Condition)
        {
            String where = null;

            switch(Condition.GetType().Name)
            {
                case "Property":

                    if (((Conditions.Property)Condition).Name == "id")
                    {
                        if (((Conditions.Property)Condition).Value == null)
                        {
                            where = "(" + ((ItemType)ItemType).TableName + ".[id] is null)";
                        }
                        else
                        {
                            return "(" + ((ItemType)ItemType).TableName + ".[id]" + OperatorString(((Conditions.Property)Condition).Operator) + "'" + ((Conditions.Property)Condition).Value.ToString() + "')";
                        }
                    }
                    else
                    {
                        PropertyType proptype = (PropertyType)ItemType.PropertyType(((Conditions.Property)Condition).Name);

                        switch (proptype.GetType().Name)
                        {
                            case "String":

                                if (((Conditions.Property)Condition).Value == null)
                                {
                                    return "(" + proptype.ColumnName + " is null)";
                                }
                                else
                                {
                                    return "(" + proptype.ColumnName + OperatorString(((Conditions.Property)Condition).Operator) + "'" + ((Conditions.Property)Condition).Value.ToString().Replace('*', '%') + "')";
                                }

                            default:
                                throw new Exceptions.ArgumentException("Property Type not implemented: " + proptype.GetType().Name);
                        }
                    }

                    break;

                case "Or":

                    switch (Condition.Children.Count())
                    {
                        case 0:
                            throw new Exceptions.ArgumentException("Invalid Or Condition, must have at leat one Child");
                        case 1:
                            where = this.Where(ItemType, Condition.Children.First());

                            break;
                        default:
                            where = "(" + this.Where(ItemType, Condition.Children.First());

                            for (int i = 1; i < Condition.Children.Count(); i++)
                            {
                                where += " or " + this.Where(ItemType, Condition.Children.ElementAt(i));
                            }

                            where += ")";

                            break;
                    }

                    break;

                case "And":

                    switch (Condition.Children.Count())
                    {
                        case 0:
                            throw new Exceptions.ArgumentException("Invalid And Condition, must have at leat one Child");
                        case 1:
                            where = this.Where(ItemType, Condition.Children.First());

                            break;
                        default:
                            where = "(" + this.Where(ItemType, Condition.Children.First());

                            for (int i = 1; i < Condition.Children.Count(); i++)
                            {
                                where += " and " + this.Where(ItemType, Condition.Children.ElementAt(i));
                            }

                            where += ")";

                            break;
                    }

                    break;

                default:
                    throw new Exceptions.ArgumentException("Condition not implemented: " + Condition.GetType().Name);
            }

            return where;
        }

        public IEnumerable<IItem> Query(String Name, Condition Condition)
        {
            return this.Query(this.ItemType(Name), Condition);
        }

        public IEnumerable<IItem> Query(IItemType ItemType, Condition Condition)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                IOM.Item iomitems = this.Innovator.newItem(ItemType.Name, "get");
                iomitems.setAttribute("select", "id");
                iomitems.setAttribute("where", this.Where(ItemType, Condition));
                iomitems = iomitems.apply();

                if (!iomitems.isError())
                {
                    List<Item> items = new List<Item>();

                    for (int i = 0; i < iomitems.getItemCount(); i++)
                    {
                        IOM.Item iomitem = iomitems.getItemByIndex(i);
                        items.Add(this.Create((ItemType)ItemType, iomitem.getID(), State.Stored));
                    }

                    return items;
                }
                else
                {
                    String errormessage = iomitems.getErrorString();

                    if (errormessage.Equals("No items of type " + ItemType.Name + " found."))
                    {
                        return new List<Item>();
                    }
                    else
                    {
                        throw new Exceptions.ReadException(errormessage);
                    }
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public Session(String URL, String Database, String Username, String Password)
        {
            this.URL = URL;
            this.Database = Database;
            this.Username = Username;
            this.Password = Password;
            this.ItemCache = new Dictionary<ItemType,Dictionary<String,Item>>();
        }
    }
}
