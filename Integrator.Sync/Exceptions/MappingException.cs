using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync.Exceptions
{
    public class MappingException : Exception
    {
        internal MappingException(String Message)
            : base(Message)
        {
        }

        internal MappingException(String Message, Exception Inner)
            : base(Message, Inner)
        {
        }
    }
}
