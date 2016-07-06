using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IRelationship : IItem
    {
        IRelationshipType RelationshipType { get; }

        IItem Source { get; }

        IItem Related { get; }

        new IRelationship Save(Boolean Unlock = true);
    }
}
