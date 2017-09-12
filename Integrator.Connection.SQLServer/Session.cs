using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Integrator.Connection.SQLServer
{
    public class Session : ISession
    {
        private const String connection = "Connection";

        public String Name { get; set; }

        private Parameters _parameters;
        public Parameters Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, new String[] { connection });
                }

                return this._parameters;
            }
        }

        public Schema.Session Schema { get; set; }

        public Log Log { get; set; }

        public void Open()
        {
            this.CheckSQLSchema();
        }

        public void Open(String Token)
        {
            this._parameters = new Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, Token);
            this.CheckSQLSchema();
        }

        internal String Connection
        {
            get
            {
                if (this.Parameters.HasParamter(connection))
                {
                    return this.Parameters.Parameter(connection).Value;
                }
                else
                {
                    throw new Exceptions.ParameterException(connection);
                }
            }
        }

        private Dictionary<Schema.ItemType, Table> TableCache;

        private void CheckSQLSchema()
        {
            this.TableCache = new Dictionary<Schema.ItemType, Table>();

            foreach(Schema.ItemType itemtype in this.Schema.ItemTypes)
            {
                this.TableCache[itemtype] = new Table(this, itemtype);
            }
        }

        internal Table Table(Schema.ItemType ItemType)
        {
            return this.TableCache[ItemType];
        }

        internal String NewID()
        {
            StringBuilder ret = new StringBuilder(32);

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            return ret.ToString();
        }

        private Dictionary<String, Item> ItemCache;

        internal Item GetItemFromCache(Schema.ItemType ItemType, String ID, String ConfigID)
        {
            if (!(ItemType is Schema.RelationshipType))
            {
                if (!this.ItemCache.ContainsKey(ID))
                {
                    Item item = new Item(this, ItemType, ID, ConfigID);
                    this.ItemCache[ID] = item;
                    return item;
                }
                else
                {
                    return this.ItemCache[ID];
                }
            }
            else
            {
                throw new Integrator.Exceptions.ArgumentException("Use GetRelationshipFromCache");
            }
        }

        internal Relationship GetRelationshipFromCache(Schema.RelationshipType RelationshipType, String ID, String ConfigID, Item Source, Item Related)
        {
            if (!this.ItemCache.ContainsKey(ID))
            {
                Relationship item = new Relationship(this, RelationshipType, ID, ConfigID, Source, Related);
                this.ItemCache[ID] = item;
                return item;
            }
            else
            {
                return (Relationship)this.ItemCache[ID];
            }
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(this);
        }

        public IItem Create(ITransaction Transaction, Schema.ItemType ItemType)
        {
            String newid = this.NewID();
            Item item = this.GetItemFromCache(ItemType, newid, newid);
            ((Transaction)Transaction).Add(item, SQLServer.Transaction.Actions.Create);
            return item;
        }

        public IItem Create(ITransaction Transaction, String ItemTypeName)
        {
            Schema.ItemType itemtype = this.Schema.ItemType(ItemTypeName);
            return this.Create(Transaction, itemtype);
        }

        public IFile Create(ITransaction Transaction, Schema.FileType FileType, String Filename)
        {
            throw new NotImplementedException();
        }

        public IFile Create(ITransaction Transaction, String FileTypeName, String Filename)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> Index(Schema.ItemType ItemType)
        {
            return this.TableCache[ItemType].Select(null);
        }

        public IEnumerable<IItem> Index(String ItemTypeName)
        {
            Schema.ItemType itemtype = this.Schema.ItemType(ItemTypeName);
            return this.Index(itemtype);
        }

        public IEnumerable<IItem> Query(Schema.ItemType ItemType, Condition Condition)
        {
            return this.TableCache[ItemType].Select(Condition);
        }

        public IEnumerable<IItem> Query(String ItemTypeName, Condition Condition)
        {
            throw new NotImplementedException();
        }

        internal IItem Get(Schema.ItemType ItemType, String ID)
        {
            IEnumerable<IItem> results = this.Query(ItemType, Integrator.Conditions.ID(ID));

            if (results.Count() == 1)
            {
                return results.First();
            }
            else
            {
                return null;
            }
        }

        public IItem Get(String ID)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {

        }

        public Session()
        {
            this.ItemCache = new Dictionary<String, Item>();

            // Set Dummy Log
            this.Log = new Integrator.Logs.Dummy();
        }
    }
}
