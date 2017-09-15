using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Transaction
    {
        public Session Session { get; private set; }

        private List<Item> Cache;

        internal void Add(Item Item)
        {
            if (!this.Cache.Contains(Item))
            {
                this.Cache.Add(Item);
            }
        }

        protected void ItemCreated(Item Item, String ID, String ConfigID)
        {
            Item.ItemCreated(ID, ConfigID);
        }

        public IEnumerable<Item> Items
        {
            get
            {
                return this.Cache;
            }
        }

        public abstract void Commit();

        public abstract void Rollback();

        public Transaction(Session Session)
        {
            this.Cache = new List<Item>();
            this.Session = Session;
        }
    }
}
