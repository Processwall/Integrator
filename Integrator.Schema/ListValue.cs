using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema
{
    public class ListValue : IEquatable<ListValue>
    {
        public List List { get; private set; }

        private XmlNode Node { get; set; }

        public String Value
        {
            get
            {
                return this.Node.Attributes["value"].Value;
            }
        }

        public String Label
        {
            get
            {
                return this.Node.Attributes["label"].Value;
            }
        }

        public bool Equals(ListValue other)
        {
            if (other != null)
            {
                return (this.Value.Equals(other.Value) && this.List.Equals(other.List));
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is ListValue)
                {
                    return this.Equals((ListValue)obj);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.List.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value;
        }

        internal ListValue(List List, XmlNode Node)
        {
            this.List = List;
            this.Node = Node;
        }
    }
}
