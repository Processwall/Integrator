using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Sync
{
    public class Parameter
    {
        public Parameters Parameters { get; private set; }

        public String Name { get; private set; }

        private String _value;
        public String Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return this.Name + ": null";
            }
            else
            {
                return this.Name + ": " + this.Value;
            }
        }

        internal Parameter(Parameters Parameters, String Name)
        {
            this.Parameters = Parameters;
            this.Name = Name;
        }
    }
}
