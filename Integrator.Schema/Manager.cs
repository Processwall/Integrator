using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Integrator.Schema
{
    public static class Manager
    {
        public static Session Load(FileInfo Filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Filename.FullName);
            return new Session(doc);
        }

        public static Session Load(String Assembly, String Resource)
        {
            Assembly assembly = System.Reflection.Assembly.Load(Assembly);
            Stream resourcestream = assembly.GetManifestResourceStream(Resource);
            XmlDocument doc = new XmlDocument();
            doc.Load(resourcestream);
            return new Integrator.Schema.Session(doc);
        }
    }
}
