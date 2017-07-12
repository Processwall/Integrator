using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema
{
    public class RelationshipType : ItemType
    {
        public ItemType Source { get; private set; }

        public ItemType Related
        {
            get
            {
                if (this.Node.Attributes["related"] != null)
                {
                    if (!String.IsNullOrEmpty(this.Node.Attributes["related"].Value))
                    {
                        return this.DataModel.ItemType(this.Node.Attributes["related"].Value);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        internal RelationshipType(ItemType Source, XmlNode Node)
            :base(Source.DataModel, Node)
        {
            this.Source = Source;
        }
    }
}
