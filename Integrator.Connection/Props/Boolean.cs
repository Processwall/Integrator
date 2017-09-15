using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Boolean : Property
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
                    if (value is System.Boolean)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type System.Boolean");
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
                if (this.Value == null)
                {
                    return null;
                }
                else
                {
                   if ((System.Boolean)this.Value)
                   {
                       return "1";
                   }
                   else
                   {
                       return "0";
                   }
                }
            }
            set
            {
                if (System.String.IsNullOrEmpty(value))
                {
                    this.Value = null;
                }
                else
                {
                    this.Value = "1".Equals(value);
                }
            }
        }

        internal Boolean(Connection.Item Item, Integrator.Schema.PropertyTypes.Boolean PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
