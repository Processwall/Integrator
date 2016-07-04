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

            IItemType partitemtype = session.ItemType("Part");
            IEnumerable<IPropertyType> proptypes = partitemtype.PropertyTypes;

        }
    }
}
