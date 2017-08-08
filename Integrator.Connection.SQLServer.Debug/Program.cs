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

            Session sqlconnection = new Session();
            sqlconnection.Schema = new Schema.Session(doc);
            sqlconnection.Parameters.Parameter("Connection").Value = "Server=localhost; Database=JDECache; User Id=infodba; Password=infodba; Trusted_Connection=True;";
            sqlconnection.Open();

            Schema.ItemType itemmastertype = sqlconnection.Schema.ItemType("ItemMaster");
            Schema.RelationshipType crossreferencetype = itemmastertype.RelationshipType("CrossReferences");

            IEnumerable<Connection.IItem> itemmasters = sqlconnection.Query(itemmastertype, Integrator.Conditions.Eq("szSecondItemNumber", "1234"));
            Connection.IItem itemmaster = null;

            if (itemmasters.Count() == 0)
            {
                itemmaster = sqlconnection.Create(itemmastertype);
                itemmaster.Property("szSecondItemNumber").Value = "1234";
                itemmaster.Property("szItemDescription").Value = "Created Description";
                itemmaster.Save();
                itemmaster.UnLock();
            }
            else
            {
                itemmaster = itemmasters.First();
                itemmaster.Lock();
                itemmaster.Property("szItemDescription").Value = "Updated Description";
                itemmaster.Save();
                itemmaster.UnLock();
            }

            IEnumerable<Connection.IRelationship> crossreferences = itemmaster.Relationships(crossreferencetype);
            Connection.IRelationship crossreference = null;

            if (crossreferences.Count() == 0)
            {
                crossreference = itemmaster.Create(crossreferencetype);
                crossreference.Property("szXrefItemNumber").Value = "6778";
                crossreference.Save();
                crossreference.UnLock();
            }
            else
            {
                crossreference = crossreferences.First();
                crossreference.Lock();
                crossreference.Property("jdDateEnding").Value = "2001/01/01";
                crossreference.Save();
                crossreference.UnLock();
            }

            sqlconnection.Close();
        }
    }
}
