using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Connection
{
    public interface IFile : IItem
    {
        String Filename { get; }

        Stream Read();

        Stream Write();

        IFile Save(Boolean Unlock = true);
    }
}
