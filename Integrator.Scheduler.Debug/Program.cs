using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Integrator.Scheduler.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Session
            String connectionstring = "user id=innovator; password=11Rose33; server=(local)\\SQL2012; Trusted_Connection=no; database=Integrator;";
            Log log = new Logs.Console();
            Session session = new Session(connectionstring, new FileInfo("C:\\dev\\Integrator\\Mapping\\Sync.xml"), log);
            
            // Add a Job
            Sync.Action action = session.Actions.First();
            Parameters parameters = action.CreateParameters();
            parameters.Parameter("Number").Value = "G10010";
            Job job = session.Create(action, parameters);
             
            // Process Pending
            session.ResetErrorJobs();
        }
    }
}
