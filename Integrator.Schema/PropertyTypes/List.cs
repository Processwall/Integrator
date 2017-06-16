using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema.PropertyTypes
{
    public class List : PropertyType
    {
        public Schema.List PropertyList
        {
            get
            {
                return this.ItemType.DataModel.List(this.Node.Attributes["list"].Value);
            }
        }

        internal List(ItemType ItemType, XmlNode Node)
            :base(ItemType, Node)
        {

        }
    }
}
