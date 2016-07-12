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

        internal new RelationshipType SubTypeFromClassification(String Classification)
        {
            if (String.IsNullOrEmpty(Classification))
            {
                return this;
            }
            else
            {
                String itemtypename = this.Name + "." + Classification.Replace('/', '.');
                return (RelationshipType)this.RelationshipType(itemtypename);
            }
        }

        private String[] _systemProperties;
        internal override String[] SystemProperties
        {
            get
            {
                if (this._systemProperties == null)
                {
                    this._systemProperties = new String[4] { "id", "classification", "source_id", "related_id" };
                }

                return this._systemProperties;
            }
        }

        internal RelationshipType(Session Session, RelationshipType Parent, String ID, String Name, Boolean CanVersion)
            : base(Session, Parent, ID, Name, CanVersion)
        {

        }
    }
}
