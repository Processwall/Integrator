using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Integer : Property, Connection.Properties.IInteger
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || (value is System.Int32))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.Int32");
                }
            }
        }

        public System.Int32? Value
        {
            get
            {
                return (System.Int32?)this.Object;
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
                    return ((System.Int32)this.Value).ToString();
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
                    this.Value = System.Int32.Parse(value);
                }
            }
        }

        internal Integer(Schema.PropertyTypes.Integer PropertyType)
            : base(PropertyType)
        {
        }
    }
}
