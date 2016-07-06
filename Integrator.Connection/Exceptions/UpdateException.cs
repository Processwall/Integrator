using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Exceptions
{
    public class UpdateException : Exception
    {
        public UpdateException(String Message)
            : base(Message)
        {
        }
    }
}
