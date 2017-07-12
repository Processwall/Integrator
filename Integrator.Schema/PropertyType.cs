using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema
{
    public abstract class PropertyType : IEquatable<PropertyType>
    {
        public ItemType ItemType { get; private set; }

        protected XmlNode Node { get; private set; }

        public String Name
        {
            get
            {
                if (this.Node.Attributes["name"] != null)
                {
                    return this.Node.Attributes["name"].Value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Property Type Name must be specified");
                }
            }
        }

        public Boolean FromConfig
        {
            get
            {
                if (this.Node.Attributes["fromconfig"] != null)
                {
                    if (String.Compare(this.Node.Attributes["fromconfig"].Value, "true", true) == 0)
                    {
                        return true;
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
        }

        public bool Equals(PropertyType other)
        {
            if (other != null)
            {
                return (this.Name.Equals(other.Name) && this.ItemType.Equals(other.ItemType));
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
                if (obj is PropertyType)
                {
                    return this.Equals((PropertyType)obj);
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
            return this.Name.GetHashCode() ^ this.ItemType.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal PropertyType(ItemType ItemType, XmlNode Node)
        {
            this.ItemType = ItemType;
            this.Node = Node;
        }
    }
}
