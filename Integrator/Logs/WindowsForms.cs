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
using System.Windows.Forms;

namespace Integrator.Logs
{
    public class WindowsForms : Log
    {
        public TextBox TextBox { get; private set; }

        protected override void Store(Message Message)
        {
            String messagestring = Message.Date.ToString("yyyy-MM-dd HH:mm:ss") + " " + Message.Level.ToString() + " " + Message.Text + Environment.NewLine;

            if (this.TextBox.InvokeRequired)
            {
                this.TextBox.Invoke(new Action(() => this.TextBox.AppendText(messagestring)));
            }
            else
            {
                this.TextBox.AppendText(messagestring);
            }
        }

        public WindowsForms(TextBox TextBox)
        {
            this.TextBox = TextBox;
        }
    }
}
