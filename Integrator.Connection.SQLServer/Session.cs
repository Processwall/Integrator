using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Integrator.Connection.SQLServer
{
    public class Session : Connection.Session
    {
        private const String connection = "Connection";

        protected override IEnumerable<String> ParameterNames
        {
            get
            {
                return new List<String>() { connection };
            }
        }

        public override void Open()
        {
            this.CheckSQLSchema();
        }

        public override void Close()
        {
       
        }

        public override Connection.Transaction BeginTransaction()
        {
            return new Transaction(this);
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

        private static void SetItemProperties(Item Item, SqlDataReader Reader, int StartIndex)
        {
            int cnt = StartIndex;

            foreach (Schema.PropertyType proptype in Item.ItemType.PropertyTypes)
            {
                if (Reader.IsDBNull(cnt))
                {
                    Item.SetProperty(proptype, null);
                }
                else
                {
                    switch (proptype.GetType().Name)
                    {
                        case "Boolean":

                            Item.SetProperty(proptype, Reader.GetBoolean(cnt));
                            break;

                        case "Date":

                            Item.SetProperty(proptype, Reader.GetDateTime(cnt));
                            break;

                        case "Decimal":

                            Item.SetProperty(proptype, Reader.GetDecimal(cnt));
                            break;

                        case "Double":

                            Item.SetProperty(proptype, Reader.GetDouble(cnt));
                            break;

                        case "Integer":

                            Item.SetProperty(proptype, Reader.GetInt32(cnt));
                            break;

                        case "String":

                            Item.SetProperty(proptype, Reader.GetString(cnt));
                            break;

                        case "Text":

                            TextReader text = Reader.GetTextReader(cnt);

                            Item.SetProperty(proptype, text.ToString());
                            break;

                        default:
                            throw new NotImplementedException("PropertyType not implemented: " + proptype.GetType().Name);
                    }
                }

                cnt++;
            }
        }

        internal IEnumerable<Connection.Item> SelectItems(Schema.ItemType ItemType, String SQL)
        {
            List<Connection.Item> items = new List<Connection.Item>();

            using (SqlConnection connection = new SqlConnection(this.Connection))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String id = reader.GetString(0);
                            String configid = reader.GetString(1);
                            Item item = this.Build(ItemType, Integrator.Connection.Item.States.Read, id, configid);
                            SetItemProperties(item, reader, 2);
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        internal IEnumerable<Relationship> SelectRelationships(Item Source, Schema.RelationshipType RelationshipType, String SQL)
        {
            List<Relationship> relationships = new List<Relationship>();

            using (SqlConnection connection = new SqlConnection(this.Connection))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String id = reader.GetString(0);
                            String configid = reader.GetString(1);
                            String sourceid = reader.GetString(2);

                            Item related = null;

                            if (!reader.IsDBNull(3))
                            {
                                String relatedid = reader.GetString(3);

                                if (RelationshipType.Related != null)
                                {
                                    if (relatedid != null)
                                    {
                                        related = this.ReadItem(RelationshipType.Related, relatedid);
                                    }
                                }
                            }

                            Relationship relationship = this.Build(RelationshipType, Integrator.Connection.Item.States.Read, id, configid, Source, related);
                            SetItemProperties(relationship, reader, 4);
                            relationships.Add(relationship);
                        }
                    }
                }
            }

            return relationships;
        }

        public override IEnumerable<Item> ReadItems(Schema.ItemType ItemType)
        {
            return this.Table(ItemType).Select(null);
        }

        public override Item ReadItem(Schema.ItemType ItemType, String ID)
        {
            Query.Conditions.ID condition = Integrator.Conditions.ID(ID);
            IEnumerable<Item> items = this.Table(ItemType).Select(condition);

            if (items.Count() > 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<Item> ReadItems(Schema.ItemType ItemType, Query.Condition Condition)
        {
            return this.Table(ItemType).Select(Condition);
        }

        public override IEnumerable<File> ReadFiles(Schema.FileType FileType)
        {
            throw new NotImplementedException();
        }

        public override File ReadFile(Schema.FileType FileType, string ID)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<File> ReadFiles(Schema.FileType FileType, Query.Condition Condition)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Relationship> ReadRelationships(Schema.RelationshipType RelationshipType, Item Source)
        {
            return this.Table(RelationshipType).Select(Source, null);
        }

        public override Relationship ReadRelationship(Schema.RelationshipType RelationshipType, Item Source, string ID)
        {
            Query.Conditions.ID condition = Integrator.Conditions.ID(ID);
            IEnumerable<Relationship> items = this.Table(RelationshipType).Select(Source, condition);

            if (items.Count() > 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public override IEnumerable<Relationship> ReadRelationships(Schema.RelationshipType RelationshipType, Item Source, Query.Condition Condition)
        {
            return this.Table(RelationshipType).Select(Source, Condition);
        }

        internal static String NewID()
        {
            StringBuilder ret = new StringBuilder(32);

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            return ret.ToString();
        }

        internal void Save(Item Item, SqlConnection Connection, SqlTransaction Transaction)
        {
            switch (Item.State)
            {
                case Item.States.Create:
                    String newid = NewID();
                    this.Table(Item.ItemType).Insert(Connection, Transaction, Item, newid, newid);
                    this.Created(Item, newid, newid);
                    break;

                case Item.States.Update:
                    this.Table(Item.ItemType).Update(Connection, Transaction, Item);
                    this.Updated(Item);
                    break;

                default:

                    break;
            }
        }

        internal void Delete(Item Item, SqlConnection Connection, SqlTransaction Transaction)
        {
            this.Table(Item.ItemType).Delete(Connection, Transaction, Item);
            this.Deleted(Item);
        }

        public Session(Schema.Session Schema, String Name, Log Log)
            :base(Schema, Name, Log)
        {
 
        }

        public Session(Schema.Session Schema, String Name, String Token, Log Log)
            : base(Schema, Name, Token, Log)
        {
     
        }
    }
}
