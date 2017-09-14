using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Integrator.Sync
{
    public class Map
    {
        public Session Session { get; private set; }

        public Integrator.Log Log
        {
            get
            {
                return this.Session.Log;
            }
        }

        internal XmlNode Node { get; private set; }

        public String Name
        {
            get
            {
                return this.Node.Attributes["name"].Value;
            }
        }

        public Connection.ISession Source
        {
            get
            {
                return this.Session.Connection(this.Node.Attributes["source"].Value);
            }
        }

        public Connection.ISession Target
        {
            get
            {
                return this.Session.Connection(this.Node.Attributes["target"].Value);
            }
        }

        private List<Maps.ItemType> _itemTypeCache;
        private List<Maps.ItemType> ItemTypeCache
        {
            get
            {
                if (this._itemTypeCache == null)
                {
                    this._itemTypeCache = new List<Maps.ItemType>();

                    foreach (XmlNode node in this.Node.SelectNodes("itemtypes/itemtype"))
                    {
                        Maps.ItemType itemtype = new Sync.Maps.ItemType(this, node);
                        this._itemTypeCache.Add(itemtype);
                    }
                }

                return this._itemTypeCache;
            }
        }

        public IEnumerable<Maps.ItemType> ItemTypes
        {
            get
            {
                return this.ItemTypeCache;
            }
        }

        public Maps.ItemType ItemTypeBySource(String Name)
        {
            foreach(Maps.ItemType itemtype in this.ItemTypes)
            {
                if (itemtype.Source.Name.Equals(Name))
                {
                    return itemtype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid Source ItemType Name");
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal Map(Session Session, XmlNode Node)
        {
            this.Session = Session;
            this.Node = Node;
        }
    }
}
