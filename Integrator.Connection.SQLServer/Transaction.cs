using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Integrator.Connection.SQLServer
{
    public class Transaction : Connection.Transaction
    {
        private SqlConnection SQLConnection;
        private SqlTransaction SQLTransaction;

        protected override void Save(Item Item)
        {
            ((Session)this.Session).Save(Item, this.SQLConnection, this.SQLTransaction);
        }

        protected override void Delete(Item Item)
        {
            ((Session)this.Session).Delete(Item, this.SQLConnection, this.SQLTransaction);
        }

        protected override void BeforeCommit()
        {
            this.SQLConnection = new SqlConnection(((Session)this.Session).Connection);
            this.SQLConnection.Open();
            this.SQLTransaction = this.SQLConnection.BeginTransaction();
        }

        protected override void AfterCommit()
        {
            this.SQLTransaction.Commit();
            this.SQLConnection.Close();
            this.SQLTransaction = null;
            this.SQLConnection = null;
        }

        protected override void BeforeRollback()
        {
            this.SQLTransaction.Rollback();
            this.SQLConnection.Close();
            this.SQLTransaction = null;
            this.SQLConnection = null;
        }

        protected override void AfterRollback()
        {
            
        }

        internal Transaction(Session Session)
            :base(Session)
        {

        }
    }
}
