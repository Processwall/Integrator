using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Exceptions
{
    public class LoginException : Exception
    {
        public LoginException(String Message)
            : base(Message)
        {
        }

        public LoginException(String Message, Exception e)
            : base(Message, e)
        {
        }
    }
}
