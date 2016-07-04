using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.PropertyTypes
{
    public class String : PropertyType, Connection.PropertyTypes.IString
    {
        public System.Int32 Length { get; private set; }

        internal String(ItemType ItemType, System.String Name, System.Int32 Length)
            : base(ItemType, Name)
        {
            this.Length = Length;
        }
    }
}
