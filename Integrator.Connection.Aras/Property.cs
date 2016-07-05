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

        public IPropertyType PropertyType { get; private set; }

        public abstract object Object { get; set; }

        internal abstract String DBValue { get; set; }

        internal Property(PropertyType PropertyType)
        {
            this.PropertyType = PropertyType;
        }
    }
}
