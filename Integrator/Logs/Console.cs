using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Logs
{
    public class Console : Log
    {
        protected override void Store(Message Message)
        {
            System.Console.WriteLine(Message.Date.ToString("yyyy-MM-dd HH:mm:ss") + " " + Message.Level.ToString() + ": " + Message.Text);
        }

        public Console()
        {

        }
    }
}
