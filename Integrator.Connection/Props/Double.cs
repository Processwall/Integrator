using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Double : Property
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
                    if (value is System.Double)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Value must be of type System.Double");
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
                    return Convert.ToString(this.Value, System.Globalization.CultureInfo.InvariantCulture);
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
                    this.Value = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal Double(Connection.Item Item, Integrator.Schema.PropertyTypes.Double PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
