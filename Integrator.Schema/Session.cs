using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Integrator.Schema
{
    public class Session : IEquatable<Session>
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
                throw new Exceptions.ArgumentException("Invalid List Name: " + Name);
            }
        }

        private Dictionary<String, ItemType> ItemTypeCache;

        private Dictionary<String, RelationshipType> RelationshipTypeCache;

        private Dictionary<String, FileType> FileTypeCache;

        internal void AddItemTypeToCache(ItemType ItemType)
        {
            if (!this.ItemTypeCache.ContainsKey(ItemType.Name))
            {
                this.ItemTypeCache[ItemType.Name] = ItemType;

                if (ItemType is FileType)
                {
                    this.FileTypeCache[ItemType.Name] = (FileType)ItemType;
                }

                if (ItemType is RelationshipType)
                {
                    this.RelationshipTypeCache[ItemType.Name] = (RelationshipType)ItemType;
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("ItemType is already in Cache: " + ItemType.Name);
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
                throw new Exceptions.ArgumentException("Invalid ItemType Name: " + Name);
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
                throw new Exceptions.ArgumentException("Invalid ItemType Name: " + Name);
            }
        }

        public IEnumerable<FileType> FileTypes
        {
            get
            {
                return this.FileTypeCache.Values;
            }
        }

        public FileType FileType(String Name)
        {
            if (this.FileTypeCache.ContainsKey(Name))
            {
                return this.FileTypeCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid FileType Name: " + Name);
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
                    throw new Exceptions.ArgumentException("Duplicate List: " + list.Name);
                }
            }

            // Load ItemTypes
            foreach (XmlNode itemtypenode in this.Node.SelectNodes("itemtypes/itemtype"))
            {
                ItemType itemtype = new ItemType(this, itemtypenode);
            }

            // Load FileTypes
            foreach (XmlNode itemtypenode in this.Node.SelectNodes("filetypes/filetype"))
            {
                FileType filetype = new FileType(this, itemtypenode);
            }
        }

        public bool Equals(Session other)
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
                if (obj is Session)
                {
                    return this.Equals((Session)obj);
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

        internal Session(XmlDocument Document)
        {
            this.ListsCache = new Dictionary<String, List>();
            this.ItemTypeCache = new Dictionary<String, ItemType>();
            this.RelationshipTypeCache = new Dictionary<String, RelationshipType>();
            this.FileTypeCache = new Dictionary<String, FileType>();
            this.Document = Document;
            this.Load();
        }
    }
}
