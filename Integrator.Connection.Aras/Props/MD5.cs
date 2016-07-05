using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class MD5 : Property, Connection.Properties.IMD5
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
                if ((value == null) || (value is System.String))
                {
                    this._object = value;
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

        internal MD5(PropertyTypes.MD5 PropertyType)
            : base(PropertyType)
        {
        }
    }
}
