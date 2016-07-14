using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Integrator.Scheduler
{
    public class Job
    {
        public enum States { Pending=0, Running=1, Error=2, Completed=3 };

        public Session Session { get; private set; }

        public Int32 ID { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime Modified { get; private set; }

        public Sync.Action Action { get; private set; }

        public Parameters Parameters { get; private set; }

        public String Message { get; private set; }

        public States Status { get; private set; }

        public void Reset()
        {
            if (this.Status == States.Error)
            {
                this.Status = States.Pending;
                this.Message = null;
                this.Modified = DateTime.UtcNow;
                this.Save();
            }
        }

        public void Execute()
        {
            this.Refresh();

            if (this.Status == States.Pending)
            {
                this.Status = States.Running;
                this.Message = null;
                this.Modified = DateTime.UtcNow;
                this.Save();

                try
                {
                    this.Action.Execute(this.Parameters);
                    this.Status = States.Completed;
                    this.Modified = DateTime.UtcNow;
                    this.Save();
                }
                catch (Exception e)
                {
                    this.Status = States.Error;
                    this.Message = e.Message;
                    this.Modified = DateTime.UtcNow;
                    this.Save();
                }
            }
        }

        private void Refresh()
        {
            this.Session.SelectJobs("where Job.id=" + this.ID);
        }

        private void Save()
        {
            using (SqlConnection connection = new SqlConnection(this.Session.ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    if (this.ID == -1)
                    {
                        // Create New Job
                        String sql = "insert into Job (action,status,created,modified,message) output inserted.id values ('" + this.Action.Name + "'," + ((int)this.Status).ToString() + ",'" + this.Created.ToString(Session.SQLDateTime) + "','" + this.Modified.ToString(Session.SQLDateTime) + "','" + this.Message + "')";

                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            this.ID = (int)command.ExecuteScalar();
                            this.Session.JobCache[this.ID] = this;
                        }

                        foreach(Parameter parameter in this.Parameters)
                        {
                            String sqlparam = "insert into Parameter (jobid,name,value) values (" + this.ID + ",'" + parameter.Name + "','" + parameter.Value + "')";

                            using (SqlCommand command = new SqlCommand(sqlparam, connection, transaction))
                            {
                                command.ExecuteScalar();
                            }
                        }

                    }
                    else
                    {
                        // Update Existing Job
                        String sql = "update Job set status=" + ((int)this.Status).ToString() + ",modified='" + this.Modified.ToString(Session.SQLDateTime) + "',message='" + this.Message + "' where id=" + this.ID.ToString();

                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        internal void Refresh(DateTime Modified, States Status)
        {
            this.Modified = Modified;
            this.Status = Status;
        }

        internal Job(Session Session, Sync.Action Action, Parameters Parameters)
        {
            this.Session = Session;
            this.ID = -1;
            this.Created = DateTime.UtcNow;
            this.Modified = this.Created;
            this.Action = Action;
            this.Parameters = Parameters;
            this.Status = States.Pending;
            this.Save();
        }

        internal Job(Session Session, Int32 ID, DateTime Created, DateTime Modified, String Action, States Status)
        {
            this.Session = Session;
            this.ID = ID;
            this.Created = Created;
            this.Modified = Modified;
            this.Action = this.Session.Sync.Action(Action);
            this.Status = Status;
            this.Parameters = this.Action.CreateParameters();
        }
    }
}
