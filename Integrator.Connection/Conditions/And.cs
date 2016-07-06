using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Conditions
{
    public class And : Condition
    {
        public void Add(Condition Condition)
        {
            this.AddChild(Condition);
        }

        public override bool Equals(Condition other)
        {
            if (other != null && other is And && (this.Children.Count() == other.Children.Count()))
            {
                Boolean ret = true;

                for (int i = 0; i < this.Children.Count(); i++)
                {
                    if (!this.Children.ElementAt(i).Equals(other.Children.ElementAt(i)))
                    {
                        ret = false;
                        break;
                    }
                }

                return ret;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int ret = 0;

            foreach (Condition child in this.Children)
            {
                ret = ret ^ child.GetHashCode();
            }

            return ret;
        }

        internal And(Condition Left, Condition Right)
            : base()
        {
            this.AddChild(Left);
            this.AddChild(Right);
        }
    }
}
