using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.Exceptions
{
    public class ParameterException : Exception
    {
        public String Parameter { get; private set; }

        public ParameterException(String Parameter)
            : base("Missing Parameter: " + Parameter)
        {
            this.Parameter = Parameter;
        }
    }
}
