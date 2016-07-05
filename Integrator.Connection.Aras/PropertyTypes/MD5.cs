using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class MD5 : PropertyType, Connection.PropertyTypes.IMD5
    {
        internal MD5(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
