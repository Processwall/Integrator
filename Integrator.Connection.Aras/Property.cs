using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras
{
    public abstract class Property : Connection.IProperty
    {
        internal Session Session
        {
            get
            {
                return ((PropertyType)this.PropertyType).ItemType.Session;
            }
        }

        internal Boolean ValueSet { get; private set; }

        public IPropertyType PropertyType { get; private set; }

        private object _object;
        public virtual object Object 
        { 
            get
            {
                return this._object;
            }
            set
            {
                this._object = value;
                this.ValueSet = true;
            }
        }

        internal abstract String DBValue { get; set; }

        public override string ToString()
        {
            if (this.Object != null)
            {
                return this.PropertyType.ToString() + ": " + this.Object.ToString();
            }
            else
            {
                return this.PropertyType.ToString() + ": null";
            }
        }

        internal Property(PropertyType PropertyType)
        {
            this.PropertyType = PropertyType;
            this.ValueSet = false;
        }
    }
}
