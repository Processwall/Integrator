using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class List : Property
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
                    if (value is Integrator.Schema.ListValue)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type Integrator.Schema.ListValue");
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
                    return ((Integrator.Schema.ListValue)this.Value).Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    this.Value = ((Integrator.Schema.PropertyTypes.List)this.PropertyType).PropertyList.Value(value);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal List(Connection.Item Item, Integrator.Schema.PropertyTypes.List PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
