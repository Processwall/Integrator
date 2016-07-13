using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Integrator.Sync
{
    public class Session
    {
        public FileInfo Filename {get; private set;}

        private XmlNode _syncNode;
        private XmlNode SyncNode
        {
            get
            {
                if (this._syncNode == null)
                {
                    try
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(this.Filename.FullName);
                        this._syncNode = document.SelectSingleNode("sync");
                    }
                    catch (Exception e)
                    {
                        throw new Exceptions.MappingException("Unbale to read Mapping File", e);
                    }

                    if (this._syncNode == null)
                    {
                        throw new Exceptions.MappingException("sync node not present in Mapping File");
                    }
                }

                return this._syncNode;
            }
        }

        private Dictionary<String, Connection.ISession> _connectionsCache;
        private Dictionary<String, Connection.ISession> ConnectionsCache
        {
            get
            {
                if (this._connectionsCache == null)
                {
                    this._connectionsCache = new Dictionary<string, Integrator.Connection.ISession>();

                    XmlNode connectionsnode = this.SyncNode.SelectSingleNode("connections");

                    if (connectionsnode == null)
                    {
                        throw new Exceptions.MappingException("connections node not present in Mapping File");
                    }

                    foreach (XmlNode connectionnode in connectionsnode.SelectNodes("connection"))
                    {
                        String name = connectionnode.Attributes["name"].Value;
                        String assemblyname = connectionnode.Attributes["assembly"].Value;
                        String classname = connectionnode.Attributes["class"].Value;
                        String token = connectionnode.Attributes["token"].Value;

                        // Create Connection
                        String assemblylocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + assemblyname + ".dll";
                        Assembly assembly = Assembly.LoadFile(assemblylocation);
                        Type classtype = assembly.GetType(classname);
                        this._connectionsCache[name] = (Connection.ISession)Activator.CreateInstance(classtype);

                        // Set Parameters
                        foreach (XmlNode parameternode in connectionnode.SelectNodes("parameters/parameter"))
                        {
                            this._connectionsCache[name].Parameter(parameternode.Attributes["name"].Value).Value = parameternode.Attributes["value"].Value;
                        }

                        // Login
                        this._connectionsCache[name].Login(token);
                    }
                }

                return this._connectionsCache;
            }
        }

        public IEnumerable<Connection.ISession> Connections
        {
            get
            {
                return this.ConnectionsCache.Values;
            }
        }

        public Connection.ISession Connection(String Name)
        {
            if (this.ConnectionsCache.ContainsKey(Name))
            {
                return this.ConnectionsCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Connection Name");
            }
        }

        public Session(FileInfo Filename)
        {
            this.Filename = Filename;
        }
    }
}
