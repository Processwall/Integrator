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
            const String password = "1234567890";
            const String salt = "0987654321";

            Parameters test1 = new Parameters(new String[] { "p1", "p2" });
            test1.Parameter("p1").Value = "p1value";
            test1.Parameter("p2").Value = "p2value";

            String token = test1.Token(password, salt);

            Parameters test2 = new Parameters(token, password, salt);

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
