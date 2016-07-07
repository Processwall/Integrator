using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IItemType : IEquatable<IItemType>
    {
        IItemType Parent { get; }

        String Name { get; }

        Boolean CanVersion { get; }

        IEnumerable<IPropertyType> PropertyTypes { get; }

        IPropertyType PropertyType(String Name);

        IEnumerable<IRelationshipType> RelationshipTypes { get; }

        IRelationshipType RelationshipType(String Name);
    }
}
