using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class Image : PropertyType, Connection.PropertyTypes.IImage
    {
        internal Image(ItemType ItemType, System.String Name)
            : base(ItemType, Name)
        {
        }
    }
}
