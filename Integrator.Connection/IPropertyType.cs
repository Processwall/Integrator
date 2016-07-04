using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IPropertyType : IEquatable<IPropertyType>
    {
        String Name { get; }
    }
}
