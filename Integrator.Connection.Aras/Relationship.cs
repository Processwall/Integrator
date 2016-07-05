using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras
{
    public class Relationship : Item, Connection.IRelationship
    {
        public IRelationshipType RelationshipType
        {
            get
            {
                return (RelationshipType)this.ItemType;
            }
        }

        public IItem Source { get; private set; }

        private IItem _related;
        public IItem Related
        {
            get
            {
                return this._related; 
            }
            set
            {
                this._related = value;
            }
        }

        internal Relationship(RelationshipType RelationshipType, String ID, Item Source, Item Related, Boolean InDatabase)
            : base(RelationshipType, ID, InDatabase)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
