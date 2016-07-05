using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Decimal : Property, Connection.Properties.IDecimal
    {
        private object _object;
        public override object Object
        {
            get
            {
                return this._object;
            }
            set
            {
                if ((value == null) || (value is System.Decimal))
                {
                    this._object = value;
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

        internal Decimal(PropertyTypes.Decimal PropertyType)
            : base(PropertyType)
        {
        }
    }
}
