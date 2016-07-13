using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
{
    public class Parameters
    {
        private Dictionary<String, Integrator.Parameter> _parameters;

        public Integrator.Parameter Parameter(String Name)
        {
            if (this._parameters.ContainsKey(Name))
            {
                return this._parameters[Name];
            }
            else
            {
                throw new Integrator.Exceptions.ArgumentException("Invalid Parameter Name");
            }
        }

        public Parameters(String[] Names)
        {
            this._parameters = new Dictionary<String, Parameter>();

            foreach (String name in Names)
            {
                this._parameters[name] = new Parameter(name);
            }
        }
    }
}
