﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface ISession : IEquatable<ISession>
    {
        IEnumerable<IItemType> ItemTypes { get; }

        IItemType ItemType(String Name);
    }
}
