using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema.PropertyTypes
{
    public class Item : PropertyType
    {
        public Schema.ItemType PropertyItemType
        {
            get
            {
                return this.ItemType.DataModel.ItemType(this.Node.Attributes["itemtype"].Value);
            }
        }

        internal Item(ItemType ItemType, XmlNode Node)
            :base(ItemType, Node)
        {

        }
    }
}
