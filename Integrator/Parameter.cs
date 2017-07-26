using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
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
                if (!this.Parameters.ReadOnly)
                {
                    this._value = value;
                }
                else
                {
                    throw new Integrator.Exceptions.ArgumentException("Read Only Parameter");
                }
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

        internal Parameter(Parameters Parameters, String Name, String Value)
        {
            this.Parameters = Parameters;
            this.Name = Name;
            this._value = Value;
        }
    }
}
