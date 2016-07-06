using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Image : Property, Connection.Properties.IImage
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || (value is System.String))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.String");
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

        internal Image(PropertyTypes.Image PropertyType)
            : base(PropertyType)
        {
        }
    }
}
