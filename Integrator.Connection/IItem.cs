using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IItem
    {
        IItemType ItemType { get; }

        String ID { get; }

        IEnumerable<IItem> Revisions { get; }

        IEnumerable<IProperty> Properties { get; }

        IEnumerable<IRelationship> Relationships(IRelationshipType RelationshipType);

        IEnumerable<IRelationship> Relationships(String Name);
    }
}
