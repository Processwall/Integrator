using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface IRelationshipType : IItemType
    {
        IItemType Source { get; }

        IItemType Related { get; }
    }
}
