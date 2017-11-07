using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Integrator.Connection.Token
{
    public class Session
    {
        public String Name { get; private set; }

        public FileInfo Filename { get; private set; }

        private Assembly _assembly;
        private Assembly Assembly
        {
            get
            {
                if (this._assembly == null)
                {
                    this._assembly = Assembly.LoadFrom(this.Filename.FullName);
                }

                return this._assembly;
            }
        }

        private List<Type> _connectionTypes;
        public IEnumerable<Type> ConnectionTypes
        {
            get
            {
                if (this._connectionTypes == null)
                {
                    this._connectionTypes = new List<Type>();

                    foreach(Type type in this.Assembly.GetTypes())
                    {
                        if ((type.BaseType != null) && (type.BaseType == typeof(Connection.Session)))
                        {
                            this._connectionTypes.Add(type);
                        }
                    }
                }

                return this._connectionTypes;
            }
        }

        public Session(String Name, FileInfo Filename)
        {
            this.Name = Name;
            this.Filename = Filename;
        }
    }
}
