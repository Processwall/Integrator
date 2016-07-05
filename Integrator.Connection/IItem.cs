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

        String ConfigID { get; }

        IEnumerable<IItem> Configurtions { get; }

        IEnumerable<IProperty> Properties { get; }

        IEnumerable<IRelationship> Relationships(IRelationshipType RelationshipType);
    }
}
