using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOM = Aras.IOM;

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

        public override void Refresh()
        {
            base.Refresh();

            if (this.Related != null)
            {
                IOM.Item iomrelated = this.Session.Innovator.newItem(this.RelationshipType.Name, "get");
                iomrelated.setAttribute("select", "related_id");
                iomrelated.setID(this.ID);
                iomrelated = iomrelated.apply();

                if (!iomrelated.isError())
                {
                    String related_id = iomrelated.getProperty("related_id");

                    if (!this.Related.ID.Equals(related_id))
                    {
                        this.Related = this.Session.Create((RelationshipType)this.RelationshipType, this.ID, State.Stored, this.Source.ID, related_id);
                    }
                }
                else
                {
                    String error_message = iomrelated.getErrorString();

                    if (error_message.Equals("No items of type " + this.RelationshipType.Name + " found."))
                    {
                        // Deleted
                        this.Status = State.Deleted;
                    }
                    else
                    {
                        throw new Exceptions.ReadException(error_message);
                    }
                }
            }
        }

        internal Relationship(RelationshipType RelationshipType, String ID, State Status, Item Source, Item Related)
            : base(RelationshipType, ID, Status)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
