using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.SQLServer
{
    public class Server : IServer
    {
        private readonly String password = "judsuusbFGHfsgFdfgsgFG";
        private readonly String salt = "hdhdkdTfQWplKJGhj";

        private Parameters _parameters;
        public Parameters Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new Parameters(new String[] { "connection" });
                }

                return this._parameters;
            }
        }

        public String Token
        {
            get
            {
                return this.Parameters.Token(password, salt);
            }
        }

        public Schema.Session Schema { get; set; }

        public ISession Login(String Token)
        {
            Parameters parameters = new Parameters(Token, password, salt);
            return new Session(parameters.Parameter("connection").Value);
        }
    }
}
