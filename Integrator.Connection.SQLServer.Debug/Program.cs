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
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourcestream = assembly.GetManifestResourceStream("Integrator.Connection.SQLServer.Debug.Schema.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(resourcestream);

            Integrator.Connection.ISession sqlconnection = new Session();
            sqlconnection.Schema = new Schema.Session(doc);
            sqlconnection.Parameters.Parameter("Connection").Value = "Server=localhost; Database=JDECache; User Id=infodba; Password=infodba; Trusted_Connection=True;";
            sqlconnection.Open();

            Schema.ItemType itemmastertype = sqlconnection.Schema.ItemType("ItemMaster");
            Schema.RelationshipType crossreferencetype = itemmastertype.RelationshipType("CrossReferences");

            ITransaction transaction = sqlconnection.BeginTransaction();

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

            sqlconnection.Close();
        }
    }
}
