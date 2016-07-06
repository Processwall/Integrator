using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Float : PropertyType, Connection.PropertyTypes.IFloat
    {
        internal Float(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
