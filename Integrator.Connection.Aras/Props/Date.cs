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
                    return ((System.DateTime)this.Value).ToString("yyyy-MM-ddTHH:mm:ss");
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
                    this.Value = System.DateTime.Parse(value);
                }
            }
        }

        internal Date(PropertyTypes.Date PropertyType)
            : base(PropertyType)
        {
        }
    }
}
