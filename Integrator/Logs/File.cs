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
using System.IO;

namespace Integrator.Logs
{
    public class File : Log
    {
        private StreamWriter Stream;

        protected override void Store(Message Message)
        {
            this.Stream.WriteLine(Message.Date.ToString("yyyy-MM-dd HH:mm:ss") + " " + Message.Level.ToString() + ": " + Message.Text);
        }

        public override void Close()
        {
            base.Close();

            if (this.Stream != null)
            {
                this.Stream.Close();
            }
        }

        public File(FileInfo Filename, Boolean Append)
        {
            if (!Filename.Directory.Exists)
            {
                Filename.Directory.Create();
            }

            this.Stream = new StreamWriter(Filename.FullName, Append);
            this.Stream.AutoFlush = true;
        }
    }
}
