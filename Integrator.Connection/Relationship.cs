using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public class Relationship : Item
    {
        public Schema.RelationshipType RelationshipType
        {
            get
            {
                return (Schema.RelationshipType)this.ItemType;
            }
        }

        public Item Source { get; private set; }

        public Item Related { get; set; }

        internal Relationship(Session Session, Schema.RelationshipType RelationshipType, States State, String ID, String ConfigID, Item Source, Item Related)
            : base(Session, RelationshipType, State, ID, ConfigID)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
