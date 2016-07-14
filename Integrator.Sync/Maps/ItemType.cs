using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Integrator.Sync.Maps
{
    public class ItemType
    {
        public Map Map { get; private set; }

        internal XmlNode Node { get; private set; }

        public Connection.IItemType Source
        {
            get
            {
                return this.Map.Source.ItemType(this.Node.Attributes["source"].Value);
            }
        }

        public Connection.IItemType Target
        {
            get
            {
                return this.Map.Target.ItemType(this.Node.Attributes["target"].Value);
            }
        }

        private List<Maps.PropertyType> _propertyTypeCache;
        private List<Maps.PropertyType> PropertyTypeCache
        {
            get
            {
                if (this._propertyTypeCache == null)
                {
                    this._propertyTypeCache = new List<Maps.PropertyType>();

                    foreach (XmlNode node in this.Node.SelectNodes("propertytypes/propertytype"))
                    {
                        Maps.PropertyType propertytype = new Sync.Maps.PropertyType(this, node);
                        this._propertyTypeCache.Add(propertytype);
                    }
                }

                return this._propertyTypeCache;
            }
        }

        public IEnumerable<Maps.PropertyType> PropertyTypes
        {
            get
            {
                return this.PropertyTypeCache;
            }
        }

        public Maps.PropertyType KeyPropertyType
        {
            get
            {
                PropertyType key = null;

                foreach(PropertyType proptype in this.PropertyTypes)
                {
                    if (proptype.Key)
                    {
                        key = proptype;
                        break;
                    }
                }

                if (key != null)
                {
                    return key;
                }
                else
                {
                    throw new Exceptions.MappingException("Key PropertyType not specified for: " + this.ToString());
                }
            }
        }

        public override string ToString()
        {
            return this.Source.Name + " -> " + this.Target.Name;
        }

        internal ItemType(Map Map, XmlNode Node)
        {
            this.Map = Map;
            this.Node = Node;
        }
    }
}
