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

            IEnumerable<IItem> parts = session.Query("Part", Integrator.Conditions.Eq("item_number", "9876"));
            IItem part1 = parts.Last();
            part1.Relationships("Part BOM").First().Delete();
        }
    }
}
