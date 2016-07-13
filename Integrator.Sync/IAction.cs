using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync
{
    public interface IAction
    {
        String Name { get; }

        Map Map { get; }

        String[] ParameterNames { get; }

        void Execute(Parameters Parameters);
    }
}
