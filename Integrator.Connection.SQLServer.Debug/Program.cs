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
using System.IO;
using System.Reflection;

namespace Integrator.Connection.SQLServer.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Schema.Session schema = Schema.Manager.Load("Integrator.Connection.SQLServer.Debug", "Integrator.Connection.SQLServer.Debug.Schema.xml");

            Logs.Console log = new Logs.Console();

            using (Integrator.Connection.Session session = new Session(schema, "Test", log))
            {
                session.Parameters.Parameter("Connection").Value = "Server=localhost; Database=JDECache; User Id=infodba; Password=infodba; Trusted_Connection=True;";
                session.Open();

                Schema.ItemType itemmastertype = session.Schema.ItemType("ItemMaster");
                Schema.RelationshipType crossreferencetype = itemmastertype.RelationshipType("CrossReferences");

                Item itemmaster = null;
                Relationship crossreference = null;

                IEnumerable<Connection.Item> itemmasters = session.ReadItems(itemmastertype, Integrator.Conditions.Eq("szSecondItemNumber", "1234"));

                if (itemmasters.Count() > 0)
                {
                    itemmaster = itemmasters.First();
                }
                else
                {
                    using (Connection.Transaction transaction = session.BeginTransaction())
                    {
                        itemmaster = session.CreateItem(itemmastertype, transaction);
                        itemmaster.SetProperty("szSecondItemNumber", "1234");
                        itemmaster.SetProperty("szItemDescription", "Created Description");

                        crossreference = session.CreateRelationship(crossreferencetype, itemmaster, null, transaction);
                        crossreference.SetProperty("szXrefItemNumber", "6778");

                        transaction.Commit();
                    }
                }

                using (Connection.Transaction transaction = session.BeginTransaction())
                {
                    session.Update(itemmaster, transaction);
                    itemmaster.SetProperty("szItemDescription", "Created Description 999");

                    transaction.Commit();
                }

                using (Connection.Transaction transaction = session.BeginTransaction())
                {
                    session.Delete(itemmaster, transaction);

                    transaction.Commit();
                }
            }
        }
    }
}
