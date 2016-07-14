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
