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

namespace Integrator.Logs
{
    public class EventLog : Log
    {
        public const String Source = "Integrator";

        private static System.Diagnostics.EventLogEntryType ConvertLevel(Log.Levels Level)
        {
            switch (Level)
            {
                case Log.Levels.FAT:
                case Log.Levels.ERR:
                    return System.Diagnostics.EventLogEntryType.Error;
                case Log.Levels.WAR:
                    return System.Diagnostics.EventLogEntryType.Warning;
                default:
                    return System.Diagnostics.EventLogEntryType.Information;
            }
        }

        protected override void Store(Message Message)
        {
            System.Diagnostics.EventLog.WriteEntry(Source, Message.Text, ConvertLevel(Message.Level));
        }

        public EventLog()
        {

        }
    }
}
