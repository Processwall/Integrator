using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Exceptions
{
    public class LoginException : Exception
    {
        public LoginException(String Message)
            : base(Message)
        {
        }
    }
}
