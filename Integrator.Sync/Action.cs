﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync
{
    public abstract class Action
    {
        public String Name { get; private set; }

        public Map Map { get; private set; }

        public Integrator.Log Log
        {
            get
            {
                return this.Map.Log;
            }
        }

        public abstract Parameters EmptyParameters();

        public abstract void Execute(Parameters Parameters);

        public override string ToString()
        {
            return this.Name;
        }

        public Action(String Name, Map Map)
        {
            this.Name = Name;
            this.Map = Map;
        }
    }
}
