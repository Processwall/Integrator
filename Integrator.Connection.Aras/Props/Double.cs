using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Double : Property, Connection.Properties.IDouble
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || (value is System.Double))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.Double");
                }
            }
        }

        public System.Double? Value
        {
            get
            {
                return (System.Double?)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        internal override System.String DBValue
        {
            get
            {
                if (this.Value == null)
                {
                    return null;
                }
                else
                {
                    return ((System.Double)this.Value).ToString();
                }
            }
            set
            {
                if (value == null)
                {
                    this.Value = null;
                }
                else
                {
                    this.Value = System.Double.Parse(value);
                }
            }
        }

        internal Double(Schema.PropertyTypes.Double PropertyType)
            : base(PropertyType)
        {
        }
    }
}
