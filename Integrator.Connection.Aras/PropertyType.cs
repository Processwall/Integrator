using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras
{
    public abstract class PropertyType : IPropertyType
    {
        internal ItemType ItemType { get; private set; }

        public System.String Name { get; private set; }

        public bool Equals(IPropertyType other)
        {
            if (other != null)
            {
                if (other is PropertyType)
                {
                    return this.Name.Equals(other.Name) && this.ItemType.Equals(((PropertyType)other).ItemType);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal PropertyType(ItemType ItemType, System.String Name)
        {
            this.ItemType = ItemType;
            this.Name = Name;
        }
    }
}
