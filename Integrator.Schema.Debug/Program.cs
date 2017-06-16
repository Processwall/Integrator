using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.IO;

namespace Integrator.Schema.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourcestream = assembly.GetManifestResourceStream("Integrator.Schema.Debug.Sample.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(resourcestream);

            DataModel datamodel = new DataModel(doc);
        }
    }
}
