using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Integrator.Connection.SQLServer
{
    public class Transaction : ITransaction
    {
        internal enum Actions { Create, Update, Delete };

        internal Session Session { get; private set; }

        private Dictionary<Item, Actions> Cache;

        internal void Add(Item Item, Actions Action)
        {
            if (!this.Cache.ContainsKey(Item))
            {
                this.Cache[Item] = Action;
            }
            else
            {
                if (this.Cache[Item] != Action)
                {
                    throw new Integrator.Exceptions.ArgumentException("Item is already a member of the Transaction");
                }
            }
        }

        public void Commit()
        {
            using (SqlConnection connection = new SqlConnection(this.Session.Connection))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach(Item item in this.Cache.Keys)
                    {
                        Actions action = this.Cache[item];

                        switch(action)
                        {
                            case Actions.Create:
                                item.Table.Insert(connection, transaction, item);
                                break;

                            case Actions.Update:
                                item.Table.Update(connection, transaction, item);
                                break;

                            case Actions.Delete:
                                throw new NotImplementedException();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        public void Rollback()
        {

        }

        internal Transaction(Session Session)
        {
            this.Session = Session;
            this.Cache = new Dictionary<Item, Actions>();
        }
    }
}
