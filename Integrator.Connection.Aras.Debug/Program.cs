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
            ISession session = new Session();
            String token = session.Token(null, "admin", "innovator");
            session.Login(token);

            foreach(Connection.Parameter parameter in session.Parameters)
            {
                switch(parameter.Name)
                {
                    case "URL":
                        parameter.Value = "http://localhost/11SP6/";
                        break;
                    case "Database":
                        parameter.Value = "Development11SP6";
                        break;

                    default:

                        break;
                }
            }

            IEnumerable<IItem> cadindex = session.Index("CAD");
            IItem cadi = cadindex.First();
   
            IEnumerable<IItem> cads = session.Query("CAD.Mechanical.Drawing", Integrator.Conditions.Eq("item_number", "1234"));
            IItem cad = cads.Last();
        }
    }
}
