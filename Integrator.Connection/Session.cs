using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Session
    {
        public const String DateTimeFomat = "yyyy-MM-ddTHH:mm:ss";

        public String Name { get; private set; }

        public Schema.Session Schema { get; set; }

        public Log Log { get; private set; }

        protected abstract IEnumerable<String> ParameterNames { get; }

        public Parameters Parameters { get; private set; }

        public abstract void Open();

        public abstract void Close();

        private Dictionary<Integrator.Schema.ItemType, Dictionary<String, Item>> ItemCache;

        public Item GetFromCache(Integrator.Schema.ItemType ItemType, String ID)
        {
            if (this.ItemCache.ContainsKey(ItemType))
            {
                if (this.ItemCache[ItemType].ContainsKey(ID))
                {
                    return this.ItemCache[ItemType][ID];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal void AddToCache(Item Item)
        {
            if (!this.ItemCache.ContainsKey(Item.ItemType))
            {
                this.ItemCache[Item.ItemType] = new Dictionary<String, Item>();
                this.ItemCache[Item.ItemType][Item.ID] = Item;
            }
            else
            {
                if (!this.ItemCache[Item.ItemType].ContainsKey(Item.ID))
                {
                    this.ItemCache[Item.ItemType][Item.ID] = Item;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Item already in Cache");
                }
            }
        }

        public abstract Transaction BeginTransaction();

        protected abstract Item Create(Schema.ItemType ItemType);

        protected abstract Item Create(Schema.ItemType ItemType, String ID, String ConfigID);

        protected abstract File Create(Schema.ItemType ItemType, String Filename);

        protected abstract File Create(Schema.ItemType ItemType, String ID, String ConfigID, String Filename);

        public Item Create(Transaction Transaction, Schema.ItemType ItemType)
        {
            Item ret = this.Create(ItemType);
            Transaction.Add(ret);
            return ret;
        }

        public Item Create(Transaction Transaction, String ItemTypeName)
        {
            Schema.ItemType itemtype = this.Schema.ItemType(ItemTypeName);
            return this.Create(Transaction, itemtype);
        }

        public File Create(Transaction Transaction, Schema.FileType FileType, String Filename)
        {
            File ret = this.Create(FileType, Filename);
            Transaction.Add(ret);
            return ret;
        }

        public File Create(Transaction Transaction, String FileTypeName, String Filename)
        {
            Schema.FileType filetype = this.Schema.FileType(FileTypeName);
            return this.Create(Transaction, filetype, Filename);
        }

        public abstract IEnumerable<Item> Index(Schema.ItemType ItemType);

        public IEnumerable<Item> Index(String ItemTypeName)
        {
            Schema.ItemType itemtype = this.Schema.ItemType(ItemTypeName);
            return this.Index(itemtype);
        }

        public abstract IEnumerable<Item> Query(Schema.ItemType ItemType, Condition Condition);

        public IEnumerable<Item> Query(String ItemTypeName, Condition Condition)
        {
            Schema.ItemType itemtype = this.Schema.ItemType(ItemTypeName);
            return this.Query(itemtype, Condition);
        }

        public abstract Item Get(String ID);

        public Session(String Name, Log Log)
        {
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Integrator.Connection.Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, this.ParameterNames);
            this.ItemCache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
        }

        public Session(String Name, String Token, Log Log)
        {
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, Token);
            this.ItemCache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
        }
    }
}
