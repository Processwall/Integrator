using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Integrator.Sync.Maps
{
    public class PropertyType
    {
        public ItemType ItemType { get; private set; }

        internal XmlNode Node { get; private set; }

        public Connection.IPropertyType Source
        {
            get
            {
                return this.ItemType.Source.PropertyType(this.Node.Attributes["source"].Value);
            }
        }

        public Connection.IPropertyType Target
        {
            get
            {
                return this.ItemType.Target.PropertyType(this.Node.Attributes["target"].Value);
            }
        }

        public Boolean Key
        {
            get
            {
                XmlAttribute keyatt = this.Node.Attributes["key"];

                if (keyatt != null)
                {
                    return "true".Equals(keyatt.Value);
                }
                else
                {
                    return false;
                }
            }
        }

        internal PropertyType(ItemType ItemType, XmlNode Node)
        {
            this.ItemType = ItemType;
            this.Node = Node;
        }
    }
}
