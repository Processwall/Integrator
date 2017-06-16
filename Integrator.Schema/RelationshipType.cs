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
                String relatedname = this.Node.Attributes["related"].Value;

                if (!String.IsNullOrEmpty(relatedname))
                {
                    return this.DataModel.ItemType(relatedname);
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
