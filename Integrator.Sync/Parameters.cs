using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Sync
{
    public class Parameters : System.Collections.Generic.IEnumerable<Parameter>
    {
        private Dictionary<String, Parameter> _parameters;

        public System.Collections.Generic.IEnumerator<Parameter> GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Parameter Parameter(String Name)
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

        public Boolean HasParamter(String Name)
        {
            return this._parameters.ContainsKey(Name);
        }

        public Parameters(String[] Names)
        {
            this._parameters = new Dictionary<String, Parameter>();

            foreach (String name in Names)
            {
                if (!this._parameters.ContainsKey(name))
                {
                    this._parameters[name] = new Parameter(this, name);
                }
                else
                {
                    throw new Integrator.Exceptions.ArgumentException("Duplicate Parameter Name");
                }
            }
        }
    }
}
