using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Integrator.Schema
{
    public class FileType : ItemType
    {

        internal FileType(DataModel DataModel, XmlNode Node)
            :base(DataModel, Node)
        {

        }
    }
}
