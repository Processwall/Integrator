using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Integer : PropertyType, Connection.PropertyTypes.IInteger
    {
        internal Integer(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
