using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Boolean : Property, Connection.Properties.IBoolean
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
                if ((value == null) || (value is System.Boolean))
                {
                    this._object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.Boolean");
                }
            }
        }

        public System.Boolean? Value
        {
            get
            {
                return (System.Boolean?)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        internal Boolean(PropertyTypes.Boolean PropertyType)
            : base(PropertyType)
        {
        }
    }
}
