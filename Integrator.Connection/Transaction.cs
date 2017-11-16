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

namespace Integrator.Connection
{
    public abstract class Transaction : IDisposable
    {
        public Session Session { get; private set; }

        public Boolean Comitted { get; private set; }

        private List<Item> Cache;

        internal void Add(Item Item)
        {
            if (!this.Cache.Contains(Item))
            {
                this.Cache.Add(Item);
            }
        }

        protected abstract void Save(Item Item);

        protected abstract void Delete(Item Item);

        protected abstract void BeforeCommit();

        protected abstract void AfterCommit();

        protected abstract void BeforeRollback();

        protected abstract void AfterRollback();

        public void Commit()
        {
            this.BeforeCommit();

            foreach(Item item in this.Cache)
            {
                switch(item.State)
                {
                    case Item.States.Create:
                    case Item.States.Update:
                        this.Save(item);
                        break;
                    case Item.States.Delete:
                        this.Delete(item);
                        break;
                    default:
                        break;
                }
            }

            this.AfterCommit();
            this.Comitted = true;
        }

        public void Rollback()
        {
            if (!this.Comitted)
            {
                this.BeforeRollback();

                this.AfterRollback();
            }
        }

        public void Dispose()
        {
            this.Rollback();
        }

        protected Transaction(Session Session)
        {
            this.Cache = new List<Item>();
            this.Comitted = false;
            this.Session = Session;
        }
    }
}
