using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Decimal : Property, Connection.Properties.IDecimal
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || (value is System.Decimal))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.Decimal");
                }
            }
        }

        public System.Decimal? Value
        {
            get
            {
                return (System.Decimal?)this.Object;
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
                    return ((System.Decimal)this.Value).ToString();
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
                    this.Value = System.Decimal.Parse(value);
                }
            }
        }

        internal Decimal(Schema.PropertyTypes.Decimal PropertyType)
            : base(PropertyType)
        {
        }
    }
}
