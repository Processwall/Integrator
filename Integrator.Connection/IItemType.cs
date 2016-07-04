using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IItemType : IEquatable<IItemType>
    {
        String Name { get; }

        IEnumerable<IPropertyType> PropertyTypes { get; }

        IEnumerable<IRelationshipType> RelationshipTypes { get; }
    }
}
