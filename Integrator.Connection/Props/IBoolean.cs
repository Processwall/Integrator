﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Properties
{
    public interface IBoolean : IProperty
    {
        System.Boolean? Value { get; set; }
    }
}
