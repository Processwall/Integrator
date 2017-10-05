using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Session : IDisposable
    {
        public const String DateTimeFomat = "yyyy-MM-ddTHH:mm:ss";

        public Schema.Session Schema { get; private set; }

        public String Name { get; private set; }

        public Log Log { get; private set; }

        protected abstract IEnumerable<String> ParameterNames { get; }

        public Parameters Parameters { get; private set; }

        public abstract void Open();

        public abstract void Close();

        private Dictionary<Schema.ItemType, Dictionary<String, Item>> Cache;

        protected Item Build(Schema.ItemType ItemType, Item.States State, String ID, String ConfigID)
        {
            Item item = null;

            if (String.IsNullOrEmpty(ID))
            {
                item = new Item(this, ItemType, State, ID, ConfigID);
            }
            else
            {
                if (this.Cache.ContainsKey(ItemType))
                {
                    if (this.Cache[ItemType].ContainsKey(ID))
                    {
                        item = this.Cache[ItemType][ID];
                    }
                    else
                    {
                        item = new Item(this, ItemType, State, ID, ConfigID);
                        this.Cache[ItemType][ID] = item;
                    }
                }
                else
                {
                    this.Cache[ItemType] = new Dictionary<String, Item>();
                    item = new Item(this, ItemType, State, ID, ConfigID);
                    this.Cache[ItemType][ID] = item;
                }
            }

            return item;
        }

        protected void Created(Item Item, String ID, String ConfigID)
        {
            if (!(String.IsNullOrEmpty(ID) && String.IsNullOrEmpty(ConfigID)))
            {
                Item.Created(ID, ConfigID);

                if (!this.Cache.ContainsKey(Item.ItemType))
                {
                    this.Cache[Item.ItemType] = new Dictionary<String, Item>();
                }

                this.Cache[Item.ItemType][ID] = Item;
            }
            else
            {
                throw new Exceptions.ArgumentException("ID and ConfigID must be specified");
            }
        }

        protected void Updated(Item Item)
        {
            Item.Updated();
        }

        protected void Deleted(Item Item)
        {
            this.Cache[Item.ItemType].Remove(Item.ID);
        }

        protected File Build(Schema.FileType FileType, Item.States State, String ID, String ConfigID)
        {
            File file = null;

            if (String.IsNullOrEmpty(ID))
            {
                file = new File(this, FileType, State, ID, ConfigID);
            }
            else
            {
                if (this.Cache.ContainsKey(FileType))
                {
                    if (this.Cache[FileType].ContainsKey(ID))
                    {
                        file = (File)this.Cache[FileType][ID];
                    }
                    else
                    {
                        file = new File(this, FileType, State, ID, ConfigID);
                        this.Cache[FileType][ID] = file;
                    }
                }
                else
                {
                    this.Cache[FileType] = new Dictionary<String, Item>();
                    file = new File(this, FileType, State, ID, ConfigID);
                    this.Cache[FileType][ID] = file;
                }
            }

            return file;
        }

        protected Relationship Build(Schema.RelationshipType RelationshipType, Item.States State, String ID, String ConfigID, Item Source, Item Related)
        {
            Relationship relationship = null;

            if (String.IsNullOrEmpty(ID))
            {
                relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
            }
            else
            {
                if (this.Cache.ContainsKey(RelationshipType))
                {
                    if (this.Cache[RelationshipType].ContainsKey(ID))
                    {
                        relationship = (Relationship)this.Cache[RelationshipType][ID];
                    }
                    else
                    {
                        relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
                        this.Cache[RelationshipType][ID] = relationship;
                    }
                }
                else
                {
                    this.Cache[RelationshipType] = new Dictionary<String, Item>();
                    relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
                    this.Cache[RelationshipType][ID] = relationship;
                }
            }

            return relationship;
        }

        public abstract Transaction BeginTransaction();

        public Item Create(Schema.ItemType ItemType, Transaction Transaction)
        {
            Item item = this.Build(ItemType, Item.States.Create, null, null);
            Transaction.Add(item);
            return item;
        }

        public File Create(Schema.FileType FileType, Transaction Transaction)
        {
            File file = this.Build(FileType, Item.States.Create, null, null);
            Transaction.Add(file);
            return file;
        }

        public Relationship Create(Schema.RelationshipType RelationshipType, Item Source, Item Related, Transaction Transaction)
        {
            Relationship relationship = this.Build(RelationshipType, Item.States.Create, null, null, Source, Related);
            Transaction.Add(relationship);
            return relationship;
        }

        public abstract IEnumerable<Item> Get(Schema.ItemType ItemType);

        public abstract Item Get(Schema.ItemType ItemType, String ID);

        public abstract IEnumerable<Item> Get(Schema.ItemType ItemType, Query.Condition Condition);

        public abstract IEnumerable<File> Get(Schema.FileType FileType);

        public abstract File Get(Schema.FileType FileType, String ID);

        public abstract IEnumerable<File> Get(Schema.FileType FileType, Query.Condition Condition);

        public abstract IEnumerable<Relationship> Get(Schema.RelationshipType RelationshipType, Item Source);

        public abstract Relationship Get(Schema.RelationshipType RelationshipType, Item Source, String ID);

        public abstract IEnumerable<Relationship> Get(Schema.RelationshipType RelationshipType, Item Source, Query.Condition Condition);

        public void Update(Item Item, Transaction Transaction)
        {
            Transaction.Add(Item);
            Item.Update();
        }

        public void Delete(Item Item, Transaction Transaction)
        {
            Transaction.Add(Item);
            Item.Delete();
        }

        public void Dispose()
        {
            this.Close();
        }

        public Session(Schema.Session Schema, String Name, Log Log)
        {
            this.Cache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
            this.Schema = Schema;
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Integrator.Connection.Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, this.ParameterNames);
        }

        public Session(Schema.Session Schema, String Name, String Token, Log Log)
        {
            this.Cache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
            this.Schema = Schema;
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, Token);
        }
    }
}
