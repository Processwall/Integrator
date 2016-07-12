using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public class Parameter
    {
        public String Name { get; private set; }

        public String Value { get; set; }

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

        public Parameter(String Name)
        {
            this.Name = Name;
        }
    }
}
