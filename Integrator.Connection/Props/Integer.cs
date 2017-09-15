using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Integer : Property
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
                    if (value is System.Int32)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type System.Int32");
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
                    this.Value = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal Integer(Connection.Item Item, Integrator.Schema.PropertyTypes.Integer PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
