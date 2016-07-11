using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Integrator.Connection;

namespace Integrator.Connection.Aras
{
    public class FileType : ItemType, IFileType
    {
        internal FileType(Session Session, String ID, String Name, Boolean CanVersion)
            :base(Session, ID, Name, CanVersion)
        {

        }
    }
}
