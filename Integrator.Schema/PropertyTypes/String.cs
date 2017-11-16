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

namespace Integrator.Schema.PropertyTypes
{
    public class String : PropertyType
    {
        private const Int32 defaultlength = 32;

        public Int32 Length
        {
            get
            {
                if (this.Node.Attributes["length"] != null)
                {
                    Int32 length = 0;

                    if (Int32.TryParse(this.Node.Attributes["length"].Value, out length))
                    {
                        return length;
                    }
                    else
                    {
                        return defaultlength;
                    }
                }
                else
                {
                    return defaultlength;
                }
            }
        }

        internal String(ItemType ItemType, XmlNode Node)
            :base(ItemType, Node)
        {

        }
    }
}
