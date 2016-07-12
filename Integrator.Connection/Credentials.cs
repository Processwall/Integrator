using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public class Credentials
    {
        public String Group { get; private set; }

        public String Username { get; private set; }

        public String Password { get; private set; }

        public Credentials(String Group, String Username, String Password)
        {
            this.Group = Group;
            this.Username = Username;
            this.Password = Password;
        }
    }
}
