using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;

namespace Integrator.Scheduler
{
    public class Session
    {
        internal const String SQLDateTime = "yyyy-MM-dd HH:mm:ss.fff";

        public Log Log { get; private set; }

        public FileInfo Filename { get; private set; }

        public String ConnectionString { get; private set; }

        private Sync.Session _sync;
        internal Sync.Session Sync
        {
            get
            {
                if (this._sync == null)
                {
                    this._sync = new Sync.Session(this.Filename, this.Log);
                }

                return this._sync;
            }
        }

        public IEnumerable<Sync.Action> Actions
        {
            get
            {
                return this.Sync.Actions;
            }
        }

        public Job Create(Sync.Action Action, Parameters Parameters)
        {
            return new Job(this, Action, Parameters);
        }

        internal Dictionary<Int32, Job> JobCache { get; private set; }

        internal IEnumerable<Job> SelectJobs(String Where)
        {
            List<Job> ret = new List<Job>();

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                String sql = "select Job.id,Job.action,Job.created,Job.modified,Job.message,Job.status,Parameter.name,Parameter.value from Job inner join Parameter on Job.id=Parameter.jobid " + Where;

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader result = command.ExecuteReader())
                    {
                        Job job = null;

                        while (result.Read())
                        {
                            Int32 id = result.GetInt32(0);
                            String action = result.GetString(1);
                            DateTime created = result.GetDateTime(2);
                            DateTime modified = result.GetDateTime(3);
                            String message = null;

                            if (!result.IsDBNull(4))
                            {
                                message = result.GetString(4);
                            }

                            Job.States status = (Job.States)Enum.ToObject(typeof(Job.States), result.GetInt32(5));
                            String paramname = result.GetString(6);
                            String paramvalue = null;

                            if (!result.IsDBNull(7))
                            {
                                paramvalue = result.GetString(7);
                            }

                            if ((job == null) || (job.ID != id))
                            {
                                if (this.JobCache.ContainsKey(id))
                                {
                                    job = this.JobCache[id];
                                    job.Refresh(modified, status);
                                }
                                else
                                {
                                    job = new Job(this, id, created, modified, action, status);
                                }

                                ret.Add(job);
                            }
                        
                            job.Parameters.Parameter(paramname).Value = paramvalue;
                        }
                    }
                }
            }

            return ret;
        }

        public void ProcessPendingJobs()
        {
            foreach(Job job in this.SelectJobs("where Job.status=0"))
            {
                job.Execute();
            }
        }

        public void ResetErrorJobs()
        {
            foreach(Job job in this.SelectJobs("where Job.status=2"))
            {
                job.Reset();
            }
        }

        private Table CheckTable(Database Database, String Name, Boolean Identity)
        {
            Table table = Database.Tables[Name];

            if (table == null)
            {
                // Create Table with id column
                table = new Table(Database, Name);
                Column col = new Column(table, "id", DataType.Int);
                col.Nullable = false;
                col.Identity = Identity;
                table.Columns.Add(col);
                table.Create();

                // Add Primary Index
                Index index = new Microsoft.SqlServer.Management.Smo.Index(table, "PK_" + Name);
                index.IndexKeyType = IndexKeyType.DriPrimaryKey;
                IndexedColumn indexcol = new IndexedColumn(index, "id");
                index.IndexedColumns.Add(indexcol);
                index.Create();
            }

            return table;
        }

        private Column CheckNVarCharColumn(Table Table, String Name, int Length, Boolean Nullable)
        {
            Column col = Table.Columns[Name];

            if (col == null)
            {
                col = new Column(Table, Name, DataType.NVarChar(Length));
                col.Nullable = Nullable;
                col.Create();
            }

            return col;
        }

        private void DeleteColumn(Table Table, String Name)
        {
            Column col = Table.Columns[Name];

            if (col != null)
            {
                col.Drop();
            }
        }

        private Column CheckIntColumn(Table Table, String Name, Boolean Nullable)
        {
            Column col = Table.Columns[Name];

            if (col == null)
            {
                col = new Column(Table, Name, DataType.Int);
                col.Nullable = Nullable;
                col.Create();
            }

            return col;
        }

        private Column CheckDateTimeColumn(Table Table, String Name, Boolean Nullable)
        {
            Column col = Table.Columns[Name];

            if (col == null)
            {
                col = new Column(Table, Name, DataType.DateTime);
                col.Nullable = Nullable;
                col.Create();
            }

            return col;
        }

        private void CheckDatabase()
        {
            // Check Tables and Columns
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                ServerConnection mgtconn = new ServerConnection(connection);
                Server mgtserver = new Server(mgtconn);

                // Get Database
                Database database = mgtserver.Databases[connection.Database];

                // Check for States Table
                Table statestable = this.CheckTable(database, "States", false);
                this.CheckNVarCharColumn(statestable, "name", 10, false);

                // Check for Jobs Table
                Table jobstable = this.CheckTable(database, "Job", true);
                this.CheckNVarCharColumn(jobstable, "action", 32, false);
                this.CheckIntColumn(jobstable, "status", false);
                this.CheckDateTimeColumn(jobstable, "created", false);
                this.CheckDateTimeColumn(jobstable, "modified", false);
                this.CheckNVarCharColumn(jobstable, "message", 255, false);

                // Check Parameter Table
                Table messagetable = this.CheckTable(database, "Parameter", true);
                this.CheckIntColumn(messagetable, "jobid", false);
                this.CheckNVarCharColumn(messagetable, "name", 32, true);
                this.CheckNVarCharColumn(messagetable, "value", 255, true);
            }

            // Check Entries to Status Table
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                String[] requiredstates = new String[4] { "Pending", "Running", "Error", "Completed" };
                String[] currentstates = new String[4] { null, null, null, null };

                using (SqlCommand command = new SqlCommand("select id,name from [States]", connection))
                {
                    using (SqlDataReader result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            int thisid = result.GetInt32(0);
                            string thisname = result.GetString(1);

                            if (thisid >= 0 && thisid <= 5)
                            {
                                currentstates[thisid] = thisname;
                            }
                        }
                    }

                    using (SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentstates[i] == null)
                            {
                                using (SqlCommand insertcommand = new SqlCommand("insert into [States] (id,name) values (" + i.ToString() + ",'" + requiredstates[i] + "')", connection, transaction))
                                {
                                    insertcommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand updatecommand = new SqlCommand("update [States] set name='" + requiredstates[i] + "' where id=" + i.ToString(), connection, transaction))
                                {
                                    updatecommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        public Session(String ConnectionString, FileInfo Filename, Log Log)
        {
            this.JobCache = new Dictionary<Int32, Job>();
            this.ConnectionString = ConnectionString;
            this.Filename = Filename;
            this.Log = Log;
            this.CheckDatabase();
        }
    }
}
