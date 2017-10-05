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

                IEnumerable<Connection.Item> itemmasters = session.Get(itemmastertype, Integrator.Conditions.Eq("szSecondItemNumber", "1234"));

                if (itemmasters.Count() > 0)
                {
                    itemmaster = itemmasters.First();
                }
                else
                {
                    using (Connection.Transaction transaction = session.BeginTransaction())
                    {
                        itemmaster = session.Create(itemmastertype, transaction);
                        itemmaster.SetProperty("szSecondItemNumber", "1234");
                        itemmaster.SetProperty("szItemDescription", "Created Description");

                        crossreference = session.Create(crossreferencetype, itemmaster, null, transaction);
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

            /*

            Schema.RelationshipType crossreferencetype = itemmastertype.RelationshipType("CrossReferences");
            IEnumerable<Connection.IItem> itemmasters = sqlconnection.Query(itemmastertype, Integrator.Conditions.Eq("szSecondItemNumber", "1234"));
            Connection.IItem itemmaster = null;

            if (itemmasters.Count() == 0)
            {
                itemmaster = sqlconnection.Create(transaction, itemmastertype);
                itemmaster.Property("szSecondItemNumber").Value = "1234";
                itemmaster.Property("szItemDescription").Value = "Created Description";
            }
            else
            {
                itemmaster = itemmasters.First();
                itemmaster.Update(transaction);
                itemmaster.Property("szItemDescription").Value = "Updated Description";
            }

            IEnumerable<Connection.IRelationship> crossreferences = itemmaster.Relationships(crossreferencetype);
            Connection.IRelationship crossreference = null;

            if (crossreferences.Count() == 0)
            {
                crossreference = itemmaster.Create(transaction, crossreferencetype);
                crossreference.Property("szXrefItemNumber").Value = "6778";
            }
            else
            {
                crossreference = crossreferences.First();
                crossreference.Update(transaction);
                crossreference.Property("jdDateEnding").Value = "2001/01/01";
            }

            transaction.Commit();

            */
        }
    }
}
