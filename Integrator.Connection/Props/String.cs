using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class String : Property
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
                    if (value is System.String)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Value must be of type System.String");
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
                return (System.String)this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

        internal String(Connection.Item Item, Integrator.Schema.PropertyTypes.String PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
