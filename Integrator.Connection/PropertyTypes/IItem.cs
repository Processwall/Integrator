﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.PropertyTypes
{
    public interface IItem : IPropertyType
    {
        IItemType PropertyItemType { get; }
    }
}
