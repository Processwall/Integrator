using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Integrator.Connection;

namespace Integrator.Connection.Aras
{
    public class FileType : ItemType, IFileType
    {
        internal FileType(Session Session, FileType Parent, String ID, String Name, Boolean CanVersion)
            :base(Session, Parent, ID, Name, CanVersion)
        {

        }
    }
}
