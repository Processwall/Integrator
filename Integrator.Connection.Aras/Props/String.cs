using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class String : Property, Connection.Properties.IString
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
                if ((value == null) || ((value is System.String) && (((System.String)value).Length <= ((PropertyTypes.String)this.PropertyType).Length)))
                {
                    this._object = value;
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

        internal String(PropertyTypes.String PropertyType)
            : base(PropertyType)
        {
        }
    }
}
