using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Boolean : PropertyType, Connection.PropertyTypes.IBoolean
    {
        internal Boolean(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
