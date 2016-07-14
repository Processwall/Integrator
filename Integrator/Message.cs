using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
{
    public class Message
    {
        public DateTime Date { get; private set; }

        public Log.Levels Level { get; private set; }

        public String Text { get; private set; }

        internal Message(Log.Levels Level, String Text)
        {
            this.Date = DateTime.Now;
            this.Level = Level;
            this.Text = Text;
        }
    }
}
