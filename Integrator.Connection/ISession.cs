using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface ISession : IEquatable<ISession>
    {
        IEnumerable<IItemType> ItemTypes { get; }

        IItemType ItemType(String Name);

        IItem Create(IItemType ItemType);

        IItem Create(String Name);

        IEnumerable<IItem> Index(IItemType ItemType);

        IEnumerable<IItem> Index(String Name);

        IEnumerable<IItem> Query(IItemType ItemType, Condition Condition);

        IEnumerable<IItem> Query(String Name, Condition Condition);
    }
}
