using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Properties
{
    public interface IInteger : IProperty
    {
        System.Int32? Value { get; set; }
    }
}
