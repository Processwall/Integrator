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
using System.Xml;

namespace Integrator.Schema
{
    public class RelationshipType : ItemType
    {
        public ItemType Source { get; private set; }

        public ItemType Related
        {
            get
            {
                if (this.Node.Attributes["related"] != null)
                {
                    if (!String.IsNullOrEmpty(this.Node.Attributes["related"].Value))
                    {
                        return this.Session.ItemType(this.Node.Attributes["related"].Value);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        List<ItemType> _relatedSubTypes;
        public IEnumerable<ItemType> RelatedSubTypes
        {
            get
            {
                if (this.Node.Attributes["relatedsubtypes"] != null)
                {
                    this._relatedSubTypes = new List<ItemType>();

                    if (!String.IsNullOrEmpty(this.Node.Attributes["relatedsubtypes"].Value))
                    {
                        String[] parts = this.Node.Attributes["relatedsubtypes"].Value.Split(new char[] { ',' });

                        foreach(String part in parts)
                        {
                            this._relatedSubTypes.Add(this.Source.Session.ItemType(part));
                        }
                    }
                }
                else
                {
                    this._relatedSubTypes = null;
                }

                return this._relatedSubTypes;
            }
        }

        


        internal RelationshipType(ItemType Source, XmlNode Node)
            :base(Source.Session, Node)
        {
            this.Source = Source;
        }
    }
}
