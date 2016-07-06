using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public enum State { Created, Stored, Updating, Deleted };

    public interface IItem : IEquatable<IItem>
    {
        IItemType ItemType { get; }

        String ID { get; }

        State Status { get; }

        IEnumerable<IItem> Revisions { get; }

        IEnumerable<IProperty> Properties { get; }

        IProperty Property(IPropertyType PropertyType);

        IProperty Property(String Name);

        IEnumerable<IRelationship> Relationships(IRelationshipType RelationshipType);

        IEnumerable<IRelationship> Relationships(String Name);

        IRelationship Create(IRelationshipType RelationshipType);

        IRelationship Create(String Name);

        IRelationship Create(IRelationshipType RelationshipType, IItem Related);

        IRelationship Create(String Name, IItem Related);

        void Refresh();

        void Lock();

        void UnLock();

        IItem Save(Boolean Unlock=true);

        void Delete();
    }
}
