﻿/*  
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

        private Dictionary<String, Connection.Session> _connectionsCache;
        private Dictionary<String, Connection.Session> ConnectionsCache
        {
            get
            {
                if (this._connectionsCache == null)
                {
                    this._connectionsCache = new Dictionary<string, Integrator.Connection.Session>();

                    XmlNode connectionsnode = this.SyncNode.SelectSingleNode("connections");

                    if (connectionsnode == null)
                    {
                        throw new Exceptions.MappingException("connections node not present in Mapping File");
                    }

                    foreach (XmlNode connectionnode in connectionsnode.SelectNodes("connection"))
                    {
                        // Get Connection Attributes
                        String name = connectionnode.Attributes["name"].Value;
                        String assemblyname = connectionnode.Attributes["assembly"].Value;
                        String classname = connectionnode.Attributes["class"].Value;
                        String token = connectionnode.Attributes["token"].Value;

                        // Get Schema Attributes
                        XmlNode schemanode = connectionnode.SelectSingleNode("schema");
                        String schemaassemblyname = schemanode.Attributes["assembly"].Value;
                        String schemaresource = schemanode.Attributes["resource"].Value;

                        // Open Schema
                        Integrator.Schema.Session schema = Integrator.Schema.Manager.Load(schemaassemblyname, schemaresource);

                        // Create Connection
                        String assemblylocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + assemblyname + ".dll";
                        Assembly assembly = Assembly.LoadFrom(assemblylocation);
                        Type classtype = assembly.GetType(classname);
                        this._connectionsCache[name] = (Connection.Session)Activator.CreateInstance(classtype, new Object[] {schema, name, token, this.Log});
                        this._connectionsCache[name].Open();
                    }
                }

                return this._connectionsCache;
            }
        }

        public IEnumerable<Connection.Session> Connections
        {
            get
            {
                return this.ConnectionsCache.Values;
            }
        }

        public Connection.Session Connection(String Name)
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
