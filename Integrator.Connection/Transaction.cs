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
