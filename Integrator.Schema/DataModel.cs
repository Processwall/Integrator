using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Integrator.Schema
{
    public class DataModel : IEquatable<DataModel>
    {
        public XmlDocument Document { get; private set; }

        private XmlNode _node;
        private XmlNode Node
        {
            get
            {
                if (this._node == null)
                {
                    this._node = this.Document.SelectSingleNode("integrator");
                }

                return this._node;
            }
        }

        public String Name
        {
            get
            {
                return this.Node.Attributes["name"].Value;
            }
        }

        private Dictionary<String, List> ListsCache;

        public IEnumerable<List> Lists
        {
            get
            {
                return this.ListsCache.Values;
            }
        }

        public List List(String Name)
        {
            if (this.ListsCache.ContainsKey(Name))
            {
                return this.ListsCache[Name];
            }
            else
            {
                throw new ArgumentException("Invalid List Name: " + Name);
            }
        }

        private Dictionary<String, ItemType> ItemTypeCache;

        internal void AddItemTypeToCache(ItemType ItemType)
        {
            if (!this.ItemTypeCache.ContainsKey(ItemType.Name))
            {
                this.ItemTypeCache[ItemType.Name] = ItemType;
            }
            else
            {
                throw new ArgumentException("ItemType is already in Cache: " + ItemType.Name);
            }
        }

        internal Boolean ItemTypeInCache(String Name)
        {
            return this.ItemTypeCache.ContainsKey(Name);
        }

        internal ItemType GetItemTypeFromCache(String Name)
        {
            if (this.ItemTypeCache.ContainsKey(Name))
            {
                return this.ItemTypeCache[Name];
            }
            else
            {
                throw new ArgumentException("Invalid ItemType Name: " + Name);
            }
        }

        public IEnumerable<ItemType> ItemTypes
        {
            get
            {
                return this.ItemTypeCache.Values;
            }
        }

        public ItemType ItemType(String Name)
        {
            if (this.ItemTypeCache.ContainsKey(Name))
            {
                return this.ItemTypeCache[Name];
            }
            else
            {
                throw new ArgumentException("Invalid ItemTypeCache Name: " + Name);
            }
        }

        private void Load()
        {
            // Load Lists
            foreach (XmlNode listnode in this.Node.SelectNodes("lists/list"))
            {
                List list = new List(this, listnode);

                if (!this.ListsCache.ContainsKey(list.Name))
                {
                    this.ListsCache[list.Name] = list;
                }
                else
                {
                    throw new ArgumentException("Duplicate List: " + list.Name);
                }
            }

            // Load ItemTypes
            foreach (XmlNode itemtypenode in this.Node.SelectNodes("itemtypes/itemtype"))
            {
                ItemType itemtype = new ItemType(this, itemtypenode);
            }
        }

        public bool Equals(DataModel other)
        {
            if (other != null)
            {
                return this.Name.Equals(other.Name);
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
                if (obj is DataModel)
                {
                    return this.Equals((DataModel)obj);
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
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public DataModel(XmlDocument Document)
        {
            this.ListsCache = new Dictionary<String, List>();
            this.ItemTypeCache = new Dictionary<String, ItemType>();
            this.Document = Document;
            this.Load();
        }
    }
}
