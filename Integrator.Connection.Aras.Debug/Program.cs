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
            IEnumerable<IProperty> props = part1.Properties;
            IEnumerable<IRelationship> partboms = part1.Relationships("Part BOM");
            part1.Refresh();

            foreach(IRelationship rel in partboms)
            {
                rel.Refresh();
            }

            IEnumerable<IProperty> props2 = part1.Properties;
            IEnumerable<IRelationship> partboms2 = part1.Relationships("Part BOM");
        }
    }
}
