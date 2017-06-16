using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Properties
{
    public interface IDouble : IProperty
    {
        System.Double? Value { get; set; }
    }
}
