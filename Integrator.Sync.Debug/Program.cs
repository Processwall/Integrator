using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Session session = new Session(new FileInfo("D:\\dev\\LM\\Mapping\\Sync.xml"));
            IEnumerable<Connection.ISession> connections = session.Connections;
        }
    }
}
