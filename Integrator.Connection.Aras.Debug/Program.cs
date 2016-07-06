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
            part1.Lock();
            part1.Property("name").Object = "Test Assembly 23";
            part1.Save();

            IItem part2 = session.Create("Part");
            part2.Property("item_number").Object = "7654";
            part2.Property("name").Object = "Test Part 22";
            part2.Save();
        }
    }
}
