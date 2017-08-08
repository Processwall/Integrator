using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IItem : IEquatable<IItem>
    {
        Schema.ItemType ItemType { get; }

        String ID { get; }

        String ConfigID { get; }

        IEnumerable<IItem> Versions { get; }

        IEnumerable<IProperty> Properties { get; }

        IProperty Property(Schema.PropertyType PropertyType);

        IProperty Property(String Name);

        IEnumerable<IRelationship> Relationships(Schema.RelationshipType RelationshipType);

        IEnumerable<IRelationship> Relationships(String Name);

        IRelationship Create(ITransaction Transaction, Schema.RelationshipType RelationshipType);

        IRelationship Create(ITransaction Transaction, String Name);

        IRelationship Create(ITransaction Transaction, Schema.RelationshipType RelationshipType, IItem Related);

        IRelationship Create(ITransaction Transaction, String Name, IItem Related);

        void Refresh();

        void Update(ITransaction Transaction);

        void Delete(ITransaction Transaction);
    }
}
