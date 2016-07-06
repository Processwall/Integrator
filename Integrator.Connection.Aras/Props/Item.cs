using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class Item : Property, Connection.Properties.IItem
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || ((value is Connection.IItem) && (((Connection.IItem)value).ItemType.Equals(((PropertyTypes.Item)this.PropertyType).PropertyItemType))))
                {
                    base.Object = value;
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
                    return this.Value.ID;
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
                    this.Value = this.Session.Create((ItemType)((PropertyTypes.Item)this.PropertyType).PropertyItemType, value, State.Stored);
                }
            }
        }

        internal Item(PropertyTypes.Item PropertyType)
            : base(PropertyType)
        {
        }
    }
}
