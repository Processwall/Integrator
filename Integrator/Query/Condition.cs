using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Query
{
    public abstract class Condition : IEquatable<Condition>
    {
        private List<Condition> _children;

        public IEnumerable<Condition> Children
        {
            get
            {
                return this._children;
            }
        }

        protected void AddChild(Condition Child)
        {
            this._children.Add(Child);
        }

        public abstract bool Equals(Condition other);

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj is Condition)
                {
                    return this.Equals((Condition)obj);
                }
                else
                {
                    return false;
                }
            }
        }

        public abstract override int GetHashCode();

        internal Condition()
        {
            this._children = new List<Condition>();
        }
    }
}
