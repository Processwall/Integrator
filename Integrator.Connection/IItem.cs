using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public enum Actions { Create, Read, Update, Delete };

    public interface IItem : IEquatable<IItem>
    {
        Schema.ItemType ItemType { get; }

        String ItemID { get; }

        String VersionID { get; }

        Actions Action { get; }

        IEnumerable<IItem> Versions { get; }

        IEnumerable<IProperty> Properties { get; }

        IProperty Property(Schema.PropertyType PropertyType);

        IProperty Property(String Name);

        IEnumerable<IRelationship> Relationships(Schema.RelationshipType RelationshipType);

        IEnumerable<IRelationship> Relationships(String Name);

        IRelationship Create(Schema.RelationshipType RelationshipType);

        IRelationship Create(String Name);

        IRelationship Create(Schema.RelationshipType RelationshipType, IItem Related);

        IRelationship Create(String Name, IItem Related);

        void Refresh();

        void Lock();

        void UnLock();

        IItem Save(Boolean Unlock=true);

        void Delete();
    }
}
