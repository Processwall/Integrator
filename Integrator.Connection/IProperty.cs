using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IProperty
    {
        IPropertyType PropertyType { get; }

        object Object { get; set; }
    }
}
