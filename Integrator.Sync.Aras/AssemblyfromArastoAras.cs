using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Sync.Aras
{
    public class AssemblyfromArastoAras : Action
    {
        private readonly String[] parameternames = new String[1] {"Number"};

        public override Integrator.Parameters CreateParameters()
        {
            return new Integrator.Parameters(parameternames);
        }

        public override void Execute(Integrator.Parameters Parameters)
        {
            Maps.ItemType itemtypemap = this.Map.ItemTypeBySource("Part");
            IEnumerable<Connection.IItem> sourceitems = this.Map.Source.Query(itemtypemap.Source, Integrator.Conditions.Eq(itemtypemap.KeyPropertyType.Source.Name, Parameters.Parameter("Number").Value));
        
        }

        public AssemblyfromArastoAras(String Name, Integrator.Sync.Map Map)
            :base(Name, Map)
        {

        }
    }
}
