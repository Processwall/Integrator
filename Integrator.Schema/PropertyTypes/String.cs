using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema.PropertyTypes
{
    public class String : PropertyType
    {
        private const Int32 defaultlength = 32;

        public Int32 Length
        {
            get
            {
                if (this.Node.Attributes["length"] != null)
                {
                    Int32 length = 0;

                    if (Int32.TryParse(this.Node.Attributes["length"].Value, out length))
                    {
                        return length;
                    }
                    else
                    {
                        return defaultlength;
                    }
                }
                else
                {
                    return defaultlength;
                }
            }
        }

        internal String(ItemType ItemType, XmlNode Node)
            :base(ItemType, Node)
        {

        }
    }
}
