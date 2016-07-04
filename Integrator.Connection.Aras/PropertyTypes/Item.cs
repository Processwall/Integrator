using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Item : PropertyType, Connection.PropertyTypes.IItem
    {
        public IItemType PropertyItemType { get; private set; }

        internal Item(ItemType ItemType, System.String Name, ItemType PropertyItemType)
            : base(ItemType, Name)
        {
            this.PropertyItemType = PropertyItemType;
        }
    }
}
