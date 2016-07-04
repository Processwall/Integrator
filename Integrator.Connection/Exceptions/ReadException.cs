using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Exceptions
{
    public class ReadException : Exception
    {
        public ReadException(String Message)
            : base(Message)
        {
        }
    }
}
