﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
        
namespace Integrator.Connection.SQLServer
{
    public class Session : ISession
    {
        private readonly String ApplicationID = "SQL Server Connection";

        private Parameters _parameters;
        public Parameters Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new Parameters(ApplicationID, System.Security.Cryptography.DataProtectionScope.LocalMachine, new String[] { "connection" });
                }

                return this._parameters;
            }
        }

        public Schema.Session Schema { get; set; }

        public void Open(String Token)
        {
            Parameters parameters = new Parameters(ApplicationID, System.Security.Cryptography.DataProtectionScope.LocalMachine, Token);
        }

        internal String Connection { get; private set; }

        public IItem Create(Schema.ItemType ItemType)
        {
            throw new NotImplementedException();
        }

        public IItem Create(String ItemTypeName)
        {
            throw new NotImplementedException();
        }

        public IFile Create(Schema.FileType FileType, String Filename)
        {
            throw new NotImplementedException();
        }

        public IFile Create(String FileTypeName, String Filename)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> Index(Schema.ItemType ItemType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> Index(String ItemTypeName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> Query(Schema.ItemType ItemType, Condition Condition)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> Query(String ItemTypeName, Condition Condition)
        {
            throw new NotImplementedException();
        }

        public IItem Get(String ID)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {

        }

        internal Session()
        {

        }
    }
}