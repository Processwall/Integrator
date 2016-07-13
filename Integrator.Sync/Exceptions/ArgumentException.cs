using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync.Exceptions
{
    public class ArgumentException : Exception
    {
        internal ArgumentException(String Message)
            : base(Message)
        {
        }
    }
}
