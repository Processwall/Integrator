using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync
{
    public class Map
    {
        public String Name { get; private set; }

        public Connection.ISession Source { get; private set; }

        public Connection.ISession Target { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

        internal Map(String Name, Connection.ISession Source, Connection.ISession Target)
        {
            this.Name = Name;
            this.Source = Source;
            this.Target = Target;
        }
    }
}
