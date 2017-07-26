using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema.PropertyTypes
{
    public class Integer : PropertyType
    {
        internal Integer (ItemType ItemType, XmlNode Node)
            :base(ItemType, Node)
        {

        }
    }
}
