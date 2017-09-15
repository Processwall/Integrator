using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Decimal : Property
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
                    if (value is System.Decimal)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type System.Decimal");
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
                    this.Value = Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal Decimal(Connection.Item Item, Integrator.Schema.PropertyTypes.Decimal PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
