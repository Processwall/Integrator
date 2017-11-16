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

        private volatile Boolean Process;

        private void ProcessQueue()
        {
            while (this.Process)
            {
                Message message = null;

                while (this.Messages.TryDequeue(out message))
                {
                    this.Store(message);
                }

                Thread.Sleep(delay);
            }
        }

        public virtual void Close()
        {
            if (this.Worker != null)
            {
                this.Process = false;
                this.Worker.Join();
            }
        }

        public void Dispose()
        {
            this.Close();
        }

        public Log()
        {
            this.Process = true;

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
