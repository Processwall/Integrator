using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Exceptions
{
    public class MappingException : Exception
    {
        public MappingException(String Message)
            : base(Message)
        {
        }

        public MappingException(String Message, Exception Inner)
            : base(Message, Inner)
        {
        }
    }
}
