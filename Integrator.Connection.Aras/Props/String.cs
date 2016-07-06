using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class String : Property, Connection.Properties.IString
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || ((value is System.String) && (((System.String)value).Length <= ((PropertyTypes.String)this.PropertyType).Length)))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.String of length less than " + ((PropertyTypes.String)this.PropertyType).Length.ToString());
                }
            }
        }

        public System.String Value
        {
            get
            {
                return (System.String)this.Object;
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
                return this.Value;

            }
            set
            {
                this.Value = value;
            }
        }

        internal String(PropertyTypes.String PropertyType)
            : base(PropertyType)
        {
        }
    }
}
