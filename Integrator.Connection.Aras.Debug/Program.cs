using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Connection.Aras.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            int bufferlength = 1024;
            int length = 0;
            byte[] buffer = new byte[bufferlength];

            ISession session = new Session("http://localhost/11SP6/", "Development11SP6", "admin", "innovator");

            IEnumerable<IItem> cads = session.Query("CAD", Integrator.Conditions.Eq("item_number", "1234"));
            IItem cad = cads.Last();
            IFile file = (IFile)cad.Property("viewable_file").Object;

            using (FileStream outstream = new FileStream("c:\\temp\\test.pdf", FileMode.Create))
            {
                using (Stream filestream = file.Read())
                {
                    while ((length = filestream.Read(buffer, 0, bufferlength)) > 0)
                    {
                        outstream.Write(buffer, 0, length);
                    }
                }
            }

            IFile newfile = session.Create("File", "sample11.pdf");

            using (Stream writestream = newfile.Write())
            {
                using (FileStream instream = new FileStream("c:\\temp\\test.pdf", FileMode.Open))
                {

                    while ((length = instream.Read(buffer, 0, bufferlength)) > 0)
                    {
                        writestream.Write(buffer, 0, length);
                    }
                }
            }

            newfile = newfile.Save();

        }
    }
}
