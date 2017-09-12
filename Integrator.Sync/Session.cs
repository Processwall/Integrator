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

        public Log Log { get; private set; }

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
                        throw new Integrator.Exceptions.MappingException("Unbale to read Mapping File", e);
                    }

                    if (this._syncNode == null)
                    {
                        throw new Integrator.Exceptions.MappingException("sync node not present in Mapping File");
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
                        Assembly assembly = Assembly.LoadFrom(assemblylocation);
                        Type classtype = assembly.GetType(classname);
                        this._connectionsCache[name] = (Connection.ISession)Activator.CreateInstance(classtype);
                        this._connectionsCache[name].Name = name;

                        // Open
                        this._connectionsCache[name].Open(token);
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

        private Dictionary<String, Map> _mapsCache;
        private Dictionary<String, Map> MapsCache
        {
            get
            {
                if (this._mapsCache == null)
                {
                    this._mapsCache = new Dictionary<String, Map>();

                    foreach (XmlNode mapnode in this.SyncNode.SelectNodes("maps/map"))
                    {
                        Map map = new Sync.Map(this, mapnode);
                        this._mapsCache[map.Name] = new Map(this, mapnode);                        
                    }
                }

                return this._mapsCache;
            }
        }

        public IEnumerable<Map> Maps
        {
            get
            {
                return this.MapsCache.Values;
            }
        }

        public Map Map(String Name)
        {
            if (this.MapsCache.ContainsKey(Name))
            {
                return this.MapsCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Map Name");
            }
        }

        private Dictionary<String, Action> _actionsCache;
        private Dictionary<String, Action> ActionsCache
        {
            get
            {
                if (this._actionsCache == null)
                {
                    this._actionsCache = new Dictionary<String, Action>();

                    foreach (XmlNode connectionnode in this.SyncNode.SelectNodes("actions/action"))
                    {
                        String name = connectionnode.Attributes["name"].Value;
                        String assemblyname = connectionnode.Attributes["assembly"].Value;
                        String classname = connectionnode.Attributes["class"].Value;
                        Map map = this.Map(connectionnode.Attributes["map"].Value);

                        // Create Action
                        String assemblylocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + assemblyname + ".dll";
                        Assembly assembly = Assembly.LoadFile(assemblylocation);
                        Type classtype = assembly.GetType(classname);
                        this._actionsCache[name] = (Action)Activator.CreateInstance(classtype, name, map);
                    }

                }

                return this._actionsCache;
            }
        }

        public IEnumerable<Action> Actions
        {
            get
            {
                return this.ActionsCache.Values;
            }
        }

        public Action Action(String Name)
        {
            if (this.ActionsCache.ContainsKey(Name))
            {
                return this.ActionsCache[Name];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid Action Name");
            }
        }

        public Session(FileInfo Filename, Log Log)
        {
            this.Filename = Filename;
            this.Log = Log;
        }
    }
}
