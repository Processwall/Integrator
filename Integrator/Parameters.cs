using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
{
    public class Parameters : System.Collections.Generic.IEnumerable<Parameter>
    {
        private Dictionary<String, Integrator.Parameter> _parameters;

        public System.Collections.Generic.IEnumerator<Parameter> GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

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
