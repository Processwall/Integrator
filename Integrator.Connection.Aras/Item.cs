using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOM = Aras.IOM;

namespace Integrator.Connection.Aras
{
    public class Item : Connection.IItem
    {
        public Connection.IItemType ItemType { get; private set; }

        public String ID { get; private set; }

        public String ConfigID { get; private set; }

        public IEnumerable<IItem> Configurtions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IProperty> Properties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IRelationship> Relationships(IRelationshipType RelationshipType)
        {

            throw new NotImplementedException();

        }

        internal Item(ItemType ItemType, String ID, String ConfigID)
        {
            this.ItemType = ItemType;
            this.ID = ID;
            this.ConfigID = ConfigID;
        }
    }
}
