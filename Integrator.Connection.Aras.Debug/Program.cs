using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Connection.Aras.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            ISession session = new Session("http://localhost/11SP6/", "Development11SP6", "admin", "innovator");

            IEnumerable<IItem> cadindex = session.Index("CAD.Mechanical.Assembly");
            IItem cadi = cadindex.First();
   
            IEnumerable<IItem> cads = session.Query("CAD.Mechanical.Drawing", Integrator.Conditions.Eq("item_number", "1234"));
            IItem cad = cads.Last();
        }
    }
}
