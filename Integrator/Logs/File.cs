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
