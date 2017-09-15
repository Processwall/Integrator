using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Property
    {
        public Item Item { get; private set; }

        public Schema.PropertyType PropertyType { get; private set; }

        private Object _value;
        public virtual Object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public abstract String DBValue { get; set; }

        public override String ToString()
        {
            if (this.Value != null)
            {
                return this.PropertyType.Name + ": " + this.Value.ToString();
            }
            else
            {
                return this.PropertyType.Name + ": null";
            }
        }

        internal Property(Item Item, Schema.PropertyType PropertyType)
        {
            this.Item = Item;
            this.PropertyType = PropertyType;
        }
    }
}
