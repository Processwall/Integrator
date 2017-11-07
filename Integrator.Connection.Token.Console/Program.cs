using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Integrator.Connection.Token.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Utility to create a Token for a Connection");
            System.Console.WriteLine();

            // Get Filename of DLL
            System.Console.Write("Enter DLL Filename: ");
            FileInfo filename = new FileInfo(System.Console.ReadLine());
            
            // Get Name of Connection
            System.Console.Write("Enter Connection Name: ");
            String name = System.Console.ReadLine();
            
            // Build Session
            Session session = new Session(name, filename);

            // Select Connection Type
            System.Console.WriteLine();
            
            int a = 1;

            foreach(Type type in session.ConnectionTypes)
            {
                System.Console.WriteLine(a.ToString() + ". " + type.FullName);
            }

            System.Console.WriteLine();
            System.Console.Write("Select Connection Type: ");
            String typenumberstring = System.Console.ReadLine();
            Int32 typenumber = -1;
            Type connectiontype = null;

            if (Int32.TryParse(typenumberstring, out typenumber))
            {
                typenumber--;

                if ((typenumber >= 0) && (typenumber < session.ConnectionTypes.Count()))
                {
                    connectiontype = session.ConnectionTypes.ElementAt(typenumber);
                }
            }

            if (connectiontype != null)
            {
                Connection.Session connection = (Connection.Session)System.Activator.CreateInstance(connectiontype, new Object[] {null, name, null});

                System.Console.WriteLine();
                System.Console.WriteLine("Enter Parameter Values: ");

                foreach (Integrator.Connection.Parameter parameter in connection.Parameters)
                {
                    System.Console.Write(" - " + parameter.Name + ": ");
                    parameter.Value = System.Console.ReadLine();
                }

                System.Console.WriteLine();
                System.Console.WriteLine(connection.Parameters.Token());
            }
            else
            {
                System.Console.WriteLine("Connection Type not selected");
            }

            System.Console.WriteLine();
            System.Console.Write("Any key to exit: ");
            System.Console.ReadKey();

        }
    }
}
