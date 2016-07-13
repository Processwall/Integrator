using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Exceptions
{
    public class DeleteException : Exception
    {
        public DeleteException(String Message)
            : base(Message)
        {
        }
    }
}
