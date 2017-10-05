using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public class File : Item
    {
        public Schema.FileType FileType
        {
            get
            {
                return (Schema.FileType)this.ItemType;
            }
        }

        internal File(Session Session, Schema.FileType FileType, States State, String ID, String ConfigID)
            : base(Session, FileType, State, ID, ConfigID)
        {

        }
    }
}
