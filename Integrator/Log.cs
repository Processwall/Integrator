using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Integrator
{
    public abstract class Log : IDisposable
    {
        private const int delay = 100;

        public enum Levels { FAT = 0, ERR = 1, WAR = 2, INF = 3, DEB = 4 };

        public Levels Level { get; set; }

        public void Add(Levels Level, String Text)
        {
            if (Level <= this.Level)
            {
                Message message = new Message(Level, Text);
                this.Messages.Enqueue(message);
            }
        }

        protected abstract void Store(Message Message);

        private Thread Worker;

        private ConcurrentQueue<Message> Messages;

        private void ProcessQueue()
        {
            while (true)
            {
                Message message = null;

                while (this.Messages.TryDequeue(out message))
                {
                    this.Store(message);
                }

                Thread.Sleep(delay);
            }
        }

        public virtual void Dispose()
        {
            // Stop Workder
            if (this.Worker != null)
            {
                this.Worker.Abort();
            }
        }

        public Log()
        {
            // Default Logging to Information
            this.Level = Levels.INF;

            // Create Message Queue
            this.Messages = new ConcurrentQueue<Message>();

            // Start Background Worker
            this.Worker = new Thread(this.ProcessQueue);
            this.Worker.IsBackground = true;
            this.Worker.Start();
        }
    }
}
