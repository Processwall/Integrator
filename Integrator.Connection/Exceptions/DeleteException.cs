using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Exceptions
{
    public class DeleteException : Exception
    {
        public DeleteException(String Message)
            : base(Message)
        {
        }
    }
}
