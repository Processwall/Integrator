using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Integer : Property, Connection.Properties.IInteger
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
                if ((value == null) || (value is System.Int32))
                {
                    this._object = value;
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

        internal Integer(PropertyTypes.Integer PropertyType)
            : base(PropertyType)
        {
        }
    }
}
