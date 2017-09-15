using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Relationship : Item
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

        public Relationship(Session Session, Schema.RelationshipType RelationshipType, Item Source, Item Related)
            : base(Session, RelationshipType)
        {
            this.Source = Source;
            this.Related = Related;
        }

        public Relationship(Session Session, Schema.RelationshipType RelationshipType, String ID, String ConfigID, Item Source, Item Related)
            : base(Session, RelationshipType, ID, ConfigID)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
