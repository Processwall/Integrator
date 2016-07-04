using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class List : PropertyType, Connection.PropertyTypes.IList
    {
        internal List(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
