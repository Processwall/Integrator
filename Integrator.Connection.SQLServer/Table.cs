using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Integrator.Connection.SQLServer
{
    internal class Table
    {
        internal Session Session { get; private set; }

        internal Schema.ItemType ItemType { get; private set; }

        internal String Name
        {
            get
            {
                return this.ItemType.Name.ToLower().Replace(' ', '_');
            }
        }

        private Dictionary<String, Column> ColumnsCache;

        internal IEnumerable<Column> Columns
        {
            get
            {
                return this.ColumnsCache.Values;
            }
        }

        internal Column Column(String Name)
        {
            return this.ColumnsCache[Name];
        }

        internal Boolean HasColumn(String Name)
        {
            return this.ColumnsCache.ContainsKey(Name);
        }

        private Boolean Exists;

        private void CheckSQLSchema()
        {
            // Load Current Schema
            this.ColumnsCache = new Dictionary<String, Column>();

            String sql = "select column_name,data_type,is_nullable,character_maximum_length from information_schema.columns where table_name='" + this.Name + "'";

            using (SqlConnection connection = new SqlConnection(this.Session.Connection))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            this.Exists = true;

                            while (reader.Read())
                            {
                                // Get Max Length
                                int maxlength = -1;

                                if (!reader.IsDBNull(3))
                                {
                                    maxlength = reader.GetInt32(3);
                                }

                                this.ColumnsCache[reader.GetString(0)] = new Column(this, reader.GetString(0), reader.GetString(1), reader.GetString(2).Equals("YES"), maxlength, false, true);
                            }
                        }
                        else
                        {
                            this.Exists = false;
                        }
                    }
                }
            }

            // Check for base Columns
            if (!this.HasColumn("id"))
            {
                this.ColumnsCache["id"] = new Column(this, "id", "nvarchar", false, 32, true, false);
            }

            if (!this.HasColumn("configid"))
            {
                this.ColumnsCache["configid"] = new Column(this, "configid", "nvarchar", false, 32, false, false);
            }

            if (this.ItemType is Schema.RelationshipType)
            {
                if (!this.HasColumn("sourceid"))
                {
                    this.ColumnsCache["sourceid"] = new Column(this, "sourceid", "nvarchar", false, 32, false, false);
                }

                if (!this.HasColumn("relatedid"))
                {
                    this.ColumnsCache["relatedid"] = new Column(this, "relatedid", "nvarchar", true, 32, false, false);
                }
            }

            // Check Columns for Properties
            foreach (Schema.PropertyType proptype in this.ItemType.PropertyTypes)
            {

                String colname = proptype.Name.ToLower();

                if (!this.HasColumn(colname))
                {
                    switch (proptype.GetType().Name)
                    {
                        case "Boolean":
                            this.ColumnsCache[colname] = new Column(this, colname, "bit", true, -1, false, false);
                            break;

                        case "Date":
                            this.ColumnsCache[colname] = new Column(this, colname, "datetime", true, -1, false, false);
                            break;

                        case "Decimal":
                            this.ColumnsCache[colname] = new Column(this, colname, "decimal", true, -1, false, false);
                            break;

                        case "Double":
                            this.ColumnsCache[colname] = new Column(this, colname, "float", true, -1, false, false);
                            break;

                        case "Integer":
                            this.ColumnsCache[colname] = new Column(this, colname, "int", true, -1, false, false);
                            break;

                        case "Item":
                            this.ColumnsCache[colname] = new Column(this, colname, "string", true, 32, false, false);
                            break;

                        case "List":
                            this.ColumnsCache[colname] = new Column(this, colname, "int", true, -1, false, false);
                            break;

                        case "String":
                            this.ColumnsCache[colname] = new Column(this, colname, "nvarchar", true, ((Schema.PropertyTypes.String)proptype).Length, false, false);
                            break;

                        case "Text":
                            this.ColumnsCache[colname] = new Column(this, colname, "text", true, -1, false, false);
                            break;

                        default:
                            throw new NotImplementedException("PropertyType not implemented: " + proptype.GetType().Name);
                    }
                }
            }

            if (this.Exists)
            {
                // Update Table

                foreach (Column col in this.Columns)
                {
                    if (!col.Exists)
                    {
                        // Add Column
                        String addcolsql = "alter table " + this.Name + " add [" + col.Name + "] " + col.Type + ";";

                        using (SqlConnection connection = new SqlConnection(this.Session.Connection))
                        {
                            connection.Open();

                            using (SqlTransaction transaction = connection.BeginTransaction())
                            {
                                using (SqlCommand command = new SqlCommand(addcolsql, connection, transaction))
                                {
                                    int res = command.ExecuteNonQuery();
                                    transaction.Commit();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Create Table
                String createsql = "create table " + this.Name + "(";

                foreach (Column col in this.Columns)
                {
                    createsql += col.SQL + ",";
                }

                createsql += ");";

                using (SqlConnection connection = new SqlConnection(this.Session.Connection))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        using (SqlCommand command = new SqlCommand(createsql, connection, transaction))
                        {
                            int res = command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                }
            }

        }

        private String ValueSQL(Schema.PropertyType PropertyType, Object Value)
        {
            if (Value == null)
            {
                return "NULL";
            }
            else
            {
                switch (PropertyType.GetType().Name)
                {
                    case "Double":
                    case "List":
                        return Value.ToString();
                    case "Item":

                        if (Value is Connection.IItem)
                        {
                            return "'" + ((Connection.IItem)Value).ID.ToString() + "'";
                        }
                        else
                        {
                            return "'" + Value.ToString() + "'";
                        }

                    case "String":
                    case "Text":
                        return "'" + Value.ToString().Replace("'", "''") + "'";
                    case "Date":
                        return "'" + ((System.DateTime)Value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'";
                    case "Boolean":

                        if ((Boolean)Value)
                        {
                            return "1";
                        }
                        else
                        {
                            return "0";
                        }

                    default:
                        throw new NotImplementedException("Invalid PropertyType: " + PropertyType.GetType().Name);
                }
            }
        }

        private String OperatorSQL(Conditions.Operators Operator)
        {
            switch (Operator)
            {
                case Conditions.Operators.eq:
                    return "=";
                case Conditions.Operators.ge:
                    return ">=";
                case Conditions.Operators.gt:
                    return ">";
                case Conditions.Operators.le:
                    return "<=";
                case Conditions.Operators.lt:
                    return "<";
                case Conditions.Operators.ne:
                    return "<>";
                default:
                    throw new NotImplementedException("Invalid Condition Operator: " + Operator);
            }
        }

        private String ConditionSQL(Condition Condition)
        {
            switch (Condition.GetType().Name)
            {
                case "And":

                    String andsql = "(";

                    for (int i = 0; i < Condition.Children.Count(); i++)
                    {
                        andsql += this.ConditionSQL(Condition.Children.ElementAt(i));

                        if (i < (Condition.Children.Count() - 1))
                        {
                            andsql += " and ";
                        }
                    }

                    andsql += ")";

                    return andsql;

                case "Or":

                    String orsql = "(";

                    for (int i = 0; i < Condition.Children.Count(); i++)
                    {
                        orsql += this.ConditionSQL(Condition.Children.ElementAt(i));

                        if (i < (Condition.Children.Count() - 1))
                        {
                            orsql += " or ";
                        }
                    }

                    orsql += ")";

                    return orsql;

                case "Property":
                    Conditions.Property propcondition = (Conditions.Property)Condition;
                    Schema.PropertyType proptype = this.ItemType.PropertyType(propcondition.Name);
                    return "(" + proptype.Name.ToLower() + this.OperatorSQL(propcondition.Operator) + this.ValueSQL(proptype, propcondition.Value) + ")";
                default:
                    throw new NotImplementedException("Condition Type not implemented: " + Condition.GetType().Name);
            }
        }

        internal String ColumnsSQL
        {
            get
            {
                String sql = "id,configid";

                if (this.ItemType is Schema.RelationshipType)
                {
                    sql += ",sourceid,relatedid";
                }

                foreach (Schema.PropertyType proptype in this.ItemType.PropertyTypes)
                {
                    sql += "," + proptype.Name.ToLower();
                }

                return sql;
            }
        }

        private void SetItemProperties(Item Item, SqlDataReader Reader, int StartIndex)
        {
            int cnt = StartIndex;

            foreach (Schema.PropertyType proptype in this.ItemType.PropertyTypes)
            {
                if (Reader.IsDBNull(cnt))
                {
                    Item.Property(proptype).Value = null;
                }
                else
                {
                    switch (proptype.GetType().Name)
                    {
                        case "Boolean":

                            Item.Property(proptype).Value = Reader.GetBoolean(cnt);
                            break;

                        case "Date":

                            Item.Property(proptype).Value = Reader.GetDateTime(cnt).ToLocalTime();
                            break;

                        case "Decimal":

                            Item.Property(proptype).Value = Reader.GetDecimal(cnt);
                            break;

                        case "Double":

                            Item.Property(proptype).Value = Reader.GetDouble(cnt);
                            break;

                        case "Integer":

                            Item.Property(proptype).Value = Reader.GetInt32(cnt);
                            break;

                        case "String":

                            Item.Property(proptype).Value = Reader.GetString(cnt);
                            break;

                        case "Text":

                            TextReader text = Reader.GetTextReader(cnt);

                            Item.Property(proptype).Value = text.ToString();
                            break;

                        default:
                            throw new NotImplementedException("PropertyType not implemented: " + proptype.GetType().Name);
                    }
                }

                cnt++;
            }
        }

        private IEnumerable<Connection.IItem> SelectItems(String SQL)
        {
            List<Connection.IItem> items = new List<Connection.IItem>();

            using (SqlConnection connection = new SqlConnection(this.Session.Connection))
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
                            Item item = this.Session.GetItemFromCache(this.ItemType, id, configid);
                            this.SetItemProperties(item, reader, 2);
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        internal IEnumerable<Relationship> SelectRelationships(Item Source, String SQL)
        {
            List<Relationship> relationships = new List<Relationship>();

            using (SqlConnection connection = new SqlConnection(this.Session.Connection))
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

                                if (((Schema.RelationshipType)this.ItemType).Related != null)
                                {
                                    if (relatedid != null)
                                    {
                                        related = (Item)this.Session.Get(((Schema.RelationshipType)this.ItemType).Related, relatedid);
                                    }
                                }
                            }

                            Relationship relationship = this.Session.GetRelationshipFromCache((Schema.RelationshipType)this.ItemType, id, configid, Source, related);

                            this.SetItemProperties(relationship, reader, 4);
                            relationships.Add(relationship);
                        }
                    }
                }
            }

            return relationships;
        }

        internal IEnumerable<IItem> Select(Condition Condition)
        {
            String sql = "select " + this.ColumnsSQL + " from " + this.Name;

            if (Condition != null)
            {
                sql += " where (" + this.ConditionSQL(Condition) + ")";
            }

            return this.SelectItems(sql);
        }

        internal void Insert(SqlConnection Connection, SqlTransaction Transaction, Item Item)
        {
            String sql = "insert into " + this.Name;
            sql += " (id,configid";

            String sqlvalues = "('" + Item.ID + "','" + Item.ConfigID + "'";

            if (this.ItemType is Schema.RelationshipType)
            {
                sql += ",sourceid,relatedid";

                if (((Relationship)Item).Related != null)
                {
                    sqlvalues += ",'" + ((Relationship)Item).Source.ID + "','" + ((Relationship)Item).Related.ID + "'";
                }
                else
                {
                    sqlvalues += ",'" + ((Relationship)Item).Source.ID + "',NULL";
                }
            }

            foreach (Schema.PropertyType proptype in this.ItemType.PropertyTypes)
            {
                IProperty property = Item.Property(proptype);
                sql += "," + property.PropertyType.Name.ToLower();

                sqlvalues += "," + this.ValueSQL(proptype, property.Value);

            }

            sql += ") values " + sqlvalues + ");";


            using (SqlCommand command = new SqlCommand(sql, Connection, Transaction))
            {
                int res = command.ExecuteNonQuery();
            }
        }

        internal void Update(SqlConnection Connection, SqlTransaction Transaction, Item Item)
        {
            String sql = "update " + this.Name;

            Boolean first = true;

            foreach (Schema.PropertyType proptype in this.ItemType.PropertyTypes)
            {
                IProperty property = Item.Property(proptype);

                if (first)
                {
                    sql += " set ";
                }
                else
                {
                    sql += ",";
                }

                sql += property.PropertyType.Name.ToLower() + "=" + this.ValueSQL(proptype, property.Value);

                first = false;
            }

            if (this.ItemType is Schema.RelationshipType)
            {
                if (((Relationship)Item).Related != null)
                {
                    sql += ",relatedid='" + ((Relationship)Item).Related.ID + "'";
                }
                else
                {
                    sql += ",relatedid=NULL";
                }
            }

            sql += " where id='" + Item.ID + "'";


            using (SqlCommand command = new SqlCommand(sql, Connection, Transaction))
            {
                int res = command.ExecuteNonQuery();
            }
        }

        internal Table(Session Session, Schema.ItemType ItemType)
        {
            this.Session = Session;
            this.ItemType = ItemType;
            this.CheckSQLSchema();
        }
    }
}
