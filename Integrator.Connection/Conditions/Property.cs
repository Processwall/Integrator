using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Conditions
{
    public enum Operators { eq, ne, lt, gt, le, ge, like };

    public class Property : Condition
    {
        public String Name { get; private set; }

        public Operators Operator { get; private set; }

        public Object Value { get; private set; }

        public override bool Equals(Condition other)
        {
            if (other != null && other is Property)
            {
                return this.Name.Equals(((Property)other).Name) && this.Operator.Equals(((Property)other).Operator) && this.Value.Equals(((Property)other).Value);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Operator.GetHashCode() ^ this.Value.GetHashCode();
        }

        internal Property(String Name, Operators Operator, Object Value)
            : base()
        {
            this.Name = Name;
            this.Operator = Operator;
            this.Value = Value;
        }
    }
}
