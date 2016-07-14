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
