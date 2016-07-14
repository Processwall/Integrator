using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync
{
    public abstract class Action
    {
        public String Name { get; private set; }

        public Map Map { get; private set; }

        public abstract Parameters CreateParameters();

        public abstract void Execute(Parameters Parameters);

        public Action(String Name, Map Map)
        {
            this.Name = Name;
            this.Map = Map;
        }
    }
}
