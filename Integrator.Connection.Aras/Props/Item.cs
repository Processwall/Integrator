using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Item : Property, Connection.Properties.IItem
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
                if ((value == null) || ((value is Connection.IItem) && (((Connection.IItem)value).ItemType.Equals(((PropertyTypes.Item)this.PropertyType).PropertyItemType))))
                {
                    this._object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be Connection.IItem of type " + ((PropertyTypes.Item)this.PropertyType).PropertyItemType.Name);
                }
            }
        }

        public Connection.IItem Value
        {
            get
            {
                return (Connection.IItem)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        internal Item(PropertyTypes.Item PropertyType)
            : base(PropertyType)
        {
        }
    }
}
