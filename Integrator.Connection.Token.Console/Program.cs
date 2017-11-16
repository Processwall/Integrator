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
