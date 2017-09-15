using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Properties
{
    public class Date : Property
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
                    if (value is System.DateTime)
                    {
                        base.Value = value;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Property Valeu must be of type System.DateTime");
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
                    return ((DateTime)this.Value).ToString(Session.DateTimeFomat);
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
                    DateTime datetimevalue;

                    if (DateTime.TryParse(value, out datetimevalue))
                    {
                        this.Value = datetimevalue;
                    }
                    else
                    {
                        throw new Integrator.Exceptions.ArgumentException("Invalid DateTime String");
                    }
                }
                else
                {
                    this.Value = null;
                }
            }
        }

        internal Date(Connection.Item Item, Integrator.Schema.PropertyTypes.Date PropertyType)
            :base(Item, PropertyType)
        {

        }
    }
}
