using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Query.Conditions
{
    public class ID : Condition
    {
        public String Value { get; private set; }

        public override bool Equals(Condition other)
        {
            if (other != null && other is ID)
            {
                return this.Value.Equals(((ID)other).Value);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        internal ID(String Value)
            : base()
        {
            this.Value = Value;
        }
    }
}
