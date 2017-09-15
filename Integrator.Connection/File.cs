using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Integrator.Connection
{
    public abstract class File : Item
    {
        public Schema.FileType FileType
        {
            get
            {
                return (Schema.FileType)this.ItemType;
            }
        }

        public String Filename { get; private set; }

        public abstract Stream Read();

        public abstract Stream Write();

        public File(Session Session, Schema.FileType FileType, String Filename)
            : base(Session, FileType)
        {
            this.Filename = Filename;
        }

        public File(Session Session, Schema.FileType FileType, String ID, String ConfigID, String Filename)
            : base(Session, FileType, ID, ConfigID)
        {
            this.Filename = Filename;
        }
    }
}
