using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Log log = new Logs.Console();
            Session session = new Session(new FileInfo("c:\\dev\\Integrator\\Mapping\\Sync.xml"), log);

            Action action = session.Action("Assembly from Aras to Aras");
            Integrator.Parameters parameters = action.CreateParameters();
            parameters.Parameter("Number").Value = "1234";
            action.Execute(parameters);
        }
    }
}
