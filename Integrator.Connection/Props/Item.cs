using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Item : Property
    {
        public override object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (value != null)
                {
                    if (value is Integrator.Connection.Item)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type Integrator.Connection.Item");
                    }
                }
                else
                {
                    base.Value = null;
                }
            }
        }

        public override System.String DBValue
        {
            get
            {
                if (this.Value != null)
                {
                    return ((Integrator.Connection.Item)this.Value).ID;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!System.String.IsNullOrEmpty(value))
                {
                    this.Value = this.Item.Session.Get(value);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal Item(Connection.Item Item, Integrator.Schema.PropertyTypes.Item PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
