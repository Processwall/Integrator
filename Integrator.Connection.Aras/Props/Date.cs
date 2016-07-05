using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Date : Property, Connection.Properties.IDate
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
                if ((value == null) || (value is System.DateTime))
                {
                    this._object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.DateTime");
                }
            }
        }

        public System.DateTime? Value
        {
            get
            {
                return (System.DateTime?)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        internal Date(PropertyTypes.Date PropertyType)
            : base(PropertyType)
        {
        }
    }
}
