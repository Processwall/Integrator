using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IRelationship : IItem
    {
        Schema.RelationshipType RelationshipType { get; }

        IItem Source { get; }

        IItem Related { get; set; }

        new IRelationship Save();

        new IEnumerable<IRelationship> Versions { get; }
    }
}
