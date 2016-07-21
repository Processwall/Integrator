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
            ISession session = new Session("Aras");
            session.Parameters.Parameter("URL").Value = "http://localhost/11SP6/";
            session.Parameters.Parameter("Database").Value = "Development11SP6";
            String token = session.Token(null, "admin", "innovator");
            session.Login(token);

            IEnumerable<IItem> cadindex = session.Index("CAD");
            IItem cadi = cadindex.First();
   
            IEnumerable<IItem> cads = session.Query("CAD.Mechanical.Drawing", Integrator.Conditions.Eq("item_number", "1234"));
            IItem cad = cads.Last();
        }
    }
}
