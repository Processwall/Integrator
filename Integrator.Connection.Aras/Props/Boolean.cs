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
                    if (this.Value == true)
                    {
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
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
                    this.Value = "1".Equals(value);
                }
            }
        }

        internal Boolean(PropertyTypes.Boolean PropertyType)
            : base(PropertyType)
        {
        }
    }
}
