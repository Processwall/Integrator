using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.SQLServer
{
    public class Relationship : Item, Connection.IRelationship
    {
        public Schema.RelationshipType RelationshipType
        {
            get
            {
                return (Schema.RelationshipType)this.ItemType;
            }
        }

        public IItem Source { get; private set; }

        public IItem Related { get; set; }

        public new IRelationship Save()
        {
            base.Save();
            return this;
        }

        public new IEnumerable<IRelationship> Versions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal Relationship(Session Session, Schema.RelationshipType RelationshipType, String ID, String ConfigID, Connection.IItem Source, Connection.IItem Related)
            : base(Session, RelationshipType, ID, ConfigID)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
