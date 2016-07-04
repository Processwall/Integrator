using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras
{
    public class RelationshipType : ItemType, IRelationshipType
    {
        private ItemType _source;
        public IItemType Source
        {
            get
            {
                return this._source;
            }
            internal set
            {
                this._source = (ItemType)value;

                if (this._source != null)
                {
                    this._source.AddRelationshipType(this);
                }
            }
        }

        private ItemType _related;
        public IItemType Related
        {
            get
            {
                return this._related;
            }
            internal set
            {
                this._related = (ItemType)value;
            }
        }

        internal RelationshipType(Session Session, String ID, String Name)
            : base(Session, ID, Name)
        {

        }
    }
}
