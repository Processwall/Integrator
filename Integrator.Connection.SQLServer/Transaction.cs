/*  
  Integrator provides a set of .NET libraries for building migration and synchronisation 
  utilities for PLM (Product Lifecycle Management) Applications.

  Copyright (C) 2017 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

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
