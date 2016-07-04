using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Date : PropertyType, Connection.PropertyTypes.IDate
    {
        internal Date(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
