/*  
  Integrator provides a set of .NET libraries for building migration and synchronisation 
  utilities for PLM (Product Lifecycle Management) Applications.

  Copyright (C) 2017 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public class Relationship : Item
    {
        public Schema.RelationshipType RelationshipType
        {
            get
            {
                return (Schema.RelationshipType)this.ItemType;
            }
        }

        public Item Source { get; private set; }

        public Item Related { get; set; }

        internal Relationship(Session Session, Schema.RelationshipType RelationshipType, States State, String ID, String ConfigID, Item Source, Item Related)
            : base(Session, RelationshipType, State, ID, ConfigID)
        {
            this.Source = Source;
            this.Related = Related;
        }
    }
}
