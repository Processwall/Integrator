using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Decimal : PropertyType, Connection.PropertyTypes.IDecimal
    {
        internal Decimal(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
