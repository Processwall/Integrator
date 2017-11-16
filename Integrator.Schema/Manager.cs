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
using System.IO;
using System.Xml;
using System.Reflection;

namespace Integrator.Schema
{
    public static class Manager
    {
        public static Session Load(FileInfo Filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Filename.FullName);
            return new Session(doc);
        }

        public static Session Load(String Assembly, String Resource)
        {
            Assembly assembly = System.Reflection.Assembly.Load(Assembly);
            Stream resourcestream = assembly.GetManifestResourceStream(Resource);
            XmlDocument doc = new XmlDocument();
            doc.Load(resourcestream);
            return new Integrator.Schema.Session(doc);
        }
    }
}
