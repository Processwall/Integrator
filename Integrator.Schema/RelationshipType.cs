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
                        return this.Session.ItemType(this.Node.Attributes["related"].Value);
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

        List<ItemType> _relatedSubTypes;
        public IEnumerable<ItemType> RelatedSubTypes
        {
            get
            {
                if (this.Node.Attributes["relatedsubtypes"] != null)
                {
                    this._relatedSubTypes = new List<ItemType>();

                    if (!String.IsNullOrEmpty(this.Node.Attributes["relatedsubtypes"].Value))
                    {
                        String[] parts = this.Node.Attributes["relatedsubtypes"].Value.Split(new char[] { ',' });

                        foreach(String part in parts)
                        {
                            this._relatedSubTypes.Add(this.Source.Session.ItemType(part));
                        }
                    }
                }
                else
                {
                    this._relatedSubTypes = null;
                }

                return this._relatedSubTypes;
            }
        }

        


        internal RelationshipType(ItemType Source, XmlNode Node)
            :base(Source.Session, Node)
        {
            this.Source = Source;
        }
    }
}
