using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.EventLog.CreateSource
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.EventLog.CreateEventSource(Integrator.Logs.EventLog.Source, "Application");
        }
    }
}
