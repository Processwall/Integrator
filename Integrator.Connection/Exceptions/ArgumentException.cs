using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Exceptions
{
    public class ArgumentException : Exception
    {
        public ArgumentException(String Message)
            : base(Message)
        {
        }
    }
}
