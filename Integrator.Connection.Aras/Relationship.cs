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

        private List<IRelationship> _versions;
        public new IEnumerable<IRelationship> Versions
        {
            get
            {
                if (this._versions == null)
                {
                    if (this.RelationshipType.CanVersion)
                    {
                        String config_id = ((Property)this.Property("config_id")).DBValue;

                        IOM.Item iomrevisions = this.Session.Innovator.newItem(((ItemType)this.ItemType).DBName, "get");
                        iomrevisions.setProperty("config_id", config_id);
                        iomrevisions.setProperty("generation", "0");
                        iomrevisions.setPropertyCondition("generation", "gt");
                        iomrevisions.setAttribute("orderBy", "generation");
                        iomrevisions.setAttribute("select", "id,source_id,related_id");
                        iomrevisions = iomrevisions.apply();

                        if (!iomrevisions.isError())
                        {
                            this._versions = new List<IRelationship>();

                            for (int i = 0; i < iomrevisions.getItemCount(); i++)
                            {
                                IOM.Item iomrevision = iomrevisions.getItemByIndex(i);
                                this._versions.Add(this.Session.Create((RelationshipType)this.RelationshipType, iomrevision.getID(), State.Stored, iomrevision.getProperty("source_id"), iomrevision.getProperty("related_id")));
                            }
                        }
                        else
                        {
                            throw new Exceptions.ReadException(iomrevisions.getErrorString());
                        }
                    }
                    else
                    {
                        this._versions = new List<IRelationship>();
                        this._versions.Add(this);
                    }
                }

                return this._versions;
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (this.Related != null)
            {
                IOM.Item iomrelated = this.Session.Innovator.newItem(((RelationshipType)this.RelationshipType).DBName, "get");
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

                    if (error_message.Equals("No items of type " + ((RelationshipType)this.RelationshipType).DBName + " found."))
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

        public new IRelationship Save(Boolean Unlock = true)
        {
            // Check Source Item
            switch (this.Source.Status)
            {
                case State.Created:
                case State.Updating:

                    break;

                case State.Stored:

                    throw new Exceptions.UpdateException("Source Item is not Locked");

                default:

                    throw new Exceptions.UpdateException("Source Item is Deleted");
            }

            String relaction = null;

            switch (this.Status)
            {
                case State.Created:
                    relaction = "add";
                    break;
                case State.Updating:
                    relaction = "update";
                    break;

                case State.Stored:

                    throw new Exceptions.UpdateException("Relationship is not Locked");

                default:

                    throw new Exceptions.UpdateException("Relationship is Deleted");
            }

            IOM.Item iomrel = this.Session.Innovator.newItem(((RelationshipType)this.RelationshipType).DBName, relaction);
            iomrel.setID(this.ID);
            iomrel.setProperty("source_id", this.Source.ID);

            foreach (IPropertyType proptype in this.ItemType.PropertyTypes)
            {
                if (((Property)this.Property(proptype)).ValueSet)
                {
                    iomrel.setProperty(proptype.Name, ((Property)this.Property(proptype)).DBValue);
                }
            }

            if (this.Related == null)
            {
                iomrel.setProperty("related_id", null);
            }
            else
            {
                iomrel.setProperty("related_id", this.Related.ID);
            }

            iomrel = iomrel.apply();

            if (!iomrel.isError())
            {
                if (iomrel.getID().Equals(this.ID))
                {
                    // Update Properties
                    foreach (Property property in this.Properties)
                    {
                        property.DBValue = iomrel.getProperty(property.PropertyType.Name);
                    }

                    this.Status = State.Updating;

                    if (Unlock)
                    {
                        // Unlock
                        this.UnLock();
                    }

                    return this;
                }
                else
                {
                    Relationship newrel = this.Session.Create((RelationshipType)this.RelationshipType, iomrel.getID(), State.Updating, iomrel.getProperty("source_id"), iomrel.getProperty("related_id"));

                    if (Unlock)
                    {
                        // Unlock
                        newrel.UnLock();
                    }

                    return newrel;
                }

            }
            else
            {
                throw new Exceptions.UpdateException(iomrel.getErrorString());
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
