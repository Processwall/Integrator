using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema
{
    public class List : IEquatable<List>
    {
        public Session DataModel { get; private set; }

        private XmlNode Node { get; set; }

        public String Name
        {
            get
            {
                return this.Node.Attributes["name"].Value;
            }
        }

        private Dictionary<String, ListValue> ListValuesCache;

        public IEnumerable<ListValue> Values
        {
            get
            {
                return this.ListValuesCache.Values;
            }
        }

        private void Load()
        {
            // Load Values
            foreach (XmlNode valuenode in this.Node.SelectNodes("values/value"))
            {
                ListValue listvalue = new ListValue(this, valuenode);

                if (!this.ListValuesCache.ContainsKey(listvalue.Value))
                {
                    this.ListValuesCache[listvalue.Value] = listvalue;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Duplicate List Value: " + this.Name + ": " + listvalue.Value);
                }
            }
        }

        public bool Equals(List other)
        {
            if (other != null)
            {
                return (this.Name.Equals(other.Name) && this.DataModel.Equals(other.DataModel));
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is List)
                {
                    return this.Equals((List)obj);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.DataModel.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal List(Session DataModel, XmlNode Node)
        {
            this.ListValuesCache = new Dictionary<String, ListValue>();
            this.DataModel = DataModel;
            this.Node = Node;
            this.Load();
        }
    }
}
