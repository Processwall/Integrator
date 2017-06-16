using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Properties
{
    public interface IItem : IProperty
    {
        Connection.IItem Value { get; set; }
    }
}
