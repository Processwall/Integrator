using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            ISession session = new Session("http://localhost/11SP6/", "Development11SP6", "admin", "innovator");

            IEnumerable<IItem> parts = session.Index("Part");
            IItem part1 = parts.Last();
            part1.Lock();
            part1.Property("name").Object = "Test Assembly";
            part1.Save();
        }
    }
}
