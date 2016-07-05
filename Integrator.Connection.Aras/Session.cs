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
                    iomitemtypes.setAttribute("select", "id,name,is_relationship");
                    iomitemtypes = iomitemtypes.apply();

                    if (!iomitemtypes.isError())
                    {
                        for (int i = 0; i < iomitemtypes.getItemCount(); i++)
                        {
                            IOM.Item iomitemtype = iomitemtypes.getItemByIndex(i);

                            if (iomitemtype.getProperty("is_relationship", "0").Equals("1"))
                            {
                                RelationshipType relationshiptype = new RelationshipType(this, iomitemtype.getID(), iomitemtype.getProperty("name"));
                                this.ItemTypeCache[relationshiptype.ID] = relationshiptype;
                            }
                            else
                            {
                                ItemType itemtype = new ItemType(this, iomitemtype.getID(), iomitemtype.getProperty("name"));
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

        internal Item Create(ItemType ItemType, String ID, String ConfigID)
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
                Item item = new Item(ItemType, ID, ConfigID);
                this.ItemCache[ItemType][ID] = item;
                return item;
            }
        }

        public IItem Create(IItemType ItemType)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                String ID = this.Innovator.getNewID();
                return this.Create((ItemType)ItemType, ID, ID);
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
                iomitems.setAttribute("select", "id,config_id");
                iomitems = iomitems.apply();

                if (!iomitems.isError())
                {
                    List<Item> items = new List<Item>();

                    for (int i = 0; i < iomitems.getItemCount(); i++)
                    {
                        IOM.Item iomitem = iomitems.getItemByIndex(i);
                        items.Add(this.Create((ItemType)ItemType, iomitem.getID(), iomitem.getProperty("config_id")));
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
