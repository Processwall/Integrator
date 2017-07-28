using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.SQLServer
{
    public class Property : Connection.IProperty
    {
        public Schema.PropertyType PropertyType { get; private set; }

        public object Value { get; set; }

        public override string ToString()
        {
            if (this.Value != null)
            {
                return this.PropertyType.Name + ": " + this.Value.ToString();
            }
            else
            {
                return this.PropertyType.Name + ": null"; 
            }
        }

        internal Property(Schema.PropertyType PropertyType)
        {
            this.PropertyType = PropertyType;
        }
    }
}
