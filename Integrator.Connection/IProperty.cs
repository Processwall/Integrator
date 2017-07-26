using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IProperty
    {
        Schema.PropertyType PropertyType { get; }

        object Value { get; set; }
    }
}
