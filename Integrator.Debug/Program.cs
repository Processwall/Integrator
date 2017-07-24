using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            String AppID = "Test Application";

            Parameters test1 = new Parameters(AppID, System.Security.Cryptography.DataProtectionScope.CurrentUser, new String[] { "p1", "p2" });
            test1.Parameter("p1").Value = "p1value";
            test1.Parameter("p2").Value = "p2value";

            String token = test1.Token();

            Parameters test2 = new Parameters(AppID, System.Security.Cryptography.DataProtectionScope.CurrentUser, token);

            foreach(Parameter param in test1)
            {
                if (!param.Value.Equals(test2.Parameter(param.Name).Value))
                {
                    Console.WriteLine("Failed");
                }
            }
        }
    }
}
