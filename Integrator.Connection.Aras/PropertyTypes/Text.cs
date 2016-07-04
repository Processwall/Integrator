using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Text : PropertyType, Connection.PropertyTypes.IText
    {
        internal Text(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
