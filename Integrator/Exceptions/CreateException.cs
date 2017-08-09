using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Exceptions
{
    public class CreateException : Exception
    {
        public CreateException(String Message)
            : base(Message)
        {
        }
    }
}
