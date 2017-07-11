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

            Session schema = new Session(doc);

            ItemType test5 = schema.ItemType("test5");
            ItemType test5parent = test5.Parent;
            ItemType test2 = test5parent.Parent;
        }
    }
}
