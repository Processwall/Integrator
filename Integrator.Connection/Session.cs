/*  
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
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public abstract class Session : IDisposable
    {
        public const String DateTimeFomat = "yyyy-MM-ddTHH:mm:ss";

        public Schema.Session Schema { get; private set; }

        public String Name { get; private set; }

        public Log Log { get; private set; }

        protected abstract IEnumerable<String> ParameterNames { get; }

        public Parameters Parameters { get; private set; }

        public abstract void Open();

        public abstract void Close();

        private Dictionary<Schema.ItemType, Dictionary<String, Item>> Cache;

        protected Item Build(Schema.ItemType ItemType, Item.States State, String ID, String ConfigID)
        {
            Item item = null;

            if (String.IsNullOrEmpty(ID))
            {
                item = new Item(this, ItemType, State, ID, ConfigID);
            }
            else
            {
                if (this.Cache.ContainsKey(ItemType))
                {
                    if (this.Cache[ItemType].ContainsKey(ID))
                    {
                        item = this.Cache[ItemType][ID];
                    }
                    else
                    {
                        item = new Item(this, ItemType, State, ID, ConfigID);
                        this.Cache[ItemType][ID] = item;
                    }
                }
                else
                {
                    this.Cache[ItemType] = new Dictionary<String, Item>();
                    item = new Item(this, ItemType, State, ID, ConfigID);
                    this.Cache[ItemType][ID] = item;
                }
            }

            return item;
        }

        protected void Created(Item Item, String ID, String ConfigID)
        {
            if (!(String.IsNullOrEmpty(ID) && String.IsNullOrEmpty(ConfigID)))
            {
                Item.Created(ID, ConfigID);

                if (!this.Cache.ContainsKey(Item.ItemType))
                {
                    this.Cache[Item.ItemType] = new Dictionary<String, Item>();
                }

                this.Cache[Item.ItemType][ID] = Item;
            }
            else
            {
                throw new Exceptions.ArgumentException("ID and ConfigID must be specified");
            }
        }

        protected void Updated(Item Item)
        {
            Item.Updated();
        }

        protected void Deleted(Item Item)
        {
            this.Cache[Item.ItemType].Remove(Item.ID);
        }

        protected File Build(Schema.FileType FileType, Item.States State, String ID, String ConfigID)
        {
            File file = null;

            if (String.IsNullOrEmpty(ID))
            {
                file = new File(this, FileType, State, ID, ConfigID);
            }
            else
            {
                if (this.Cache.ContainsKey(FileType))
                {
                    if (this.Cache[FileType].ContainsKey(ID))
                    {
                        file = (File)this.Cache[FileType][ID];
                    }
                    else
                    {
                        file = new File(this, FileType, State, ID, ConfigID);
                        this.Cache[FileType][ID] = file;
                    }
                }
                else
                {
                    this.Cache[FileType] = new Dictionary<String, Item>();
                    file = new File(this, FileType, State, ID, ConfigID);
                    this.Cache[FileType][ID] = file;
                }
            }

            return file;
        }

        protected Relationship Build(Schema.RelationshipType RelationshipType, Item.States State, String ID, String ConfigID, Item Source, Item Related)
        {
            Relationship relationship = null;

            if (String.IsNullOrEmpty(ID))
            {
                relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
            }
            else
            {
                if (this.Cache.ContainsKey(RelationshipType))
                {
                    if (this.Cache[RelationshipType].ContainsKey(ID))
                    {
                        relationship = (Relationship)this.Cache[RelationshipType][ID];
                    }
                    else
                    {
                        relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
                        this.Cache[RelationshipType][ID] = relationship;
                    }
                }
                else
                {
                    this.Cache[RelationshipType] = new Dictionary<String, Item>();
                    relationship = new Relationship(this, RelationshipType, State, ID, ConfigID, Source, Related);
                    this.Cache[RelationshipType][ID] = relationship;
                }
            }

            return relationship;
        }

        public abstract Transaction BeginTransaction();

        public Item CreateItem(Schema.ItemType ItemType, Transaction Transaction)
        {
            Item item = this.Build(ItemType, Connection.Item.States.Create, null, null);
            Transaction.Add(item);
            return item;
        }

        public Item CreateItem(String ItemTypeName, Transaction Transaction)
        {
            Schema.ItemType ItemType = this.Schema.ItemType(ItemTypeName);
            return this.CreateItem(ItemType, Transaction);
        }

        public File CreateFile(Schema.FileType FileType, Transaction Transaction)
        {
            File file = this.Build(FileType, Connection.Item.States.Create, null, null);
            Transaction.Add(file);
            return file;
        }

        public File CreateFile(String FileTypeName, Transaction Transaction)
        {
            Schema.FileType FileType = this.Schema.FileType(FileTypeName);
            return this.CreateFile(FileType, Transaction);
        }

        public Relationship CreateRelationship(Schema.RelationshipType RelationshipType, Item Source, Item Related, Transaction Transaction)
        {
            Relationship relationship = this.Build(RelationshipType, Connection.Item.States.Create, null, null, Source, Related);
            Transaction.Add(relationship);
            return relationship;
        }

        public Relationship CreateRelationship(String RelationshipTypeName, Item Source, Item Related, Transaction Transaction)
        {
            Schema.RelationshipType RelationshipType = Source.ItemType.RelationshipType(RelationshipTypeName);
            return this.CreateRelationship(RelationshipType, Source, Related, Transaction);
        }

        public abstract IEnumerable<Item> ReadItems(Schema.ItemType ItemType);

        public IEnumerable<Item> ReadItems(String ItemTypeName)
        {
            Schema.ItemType ItemType = this.Schema.ItemType(ItemTypeName);
            return this.ReadItems(ItemType);
        }

        public abstract Item ReadItem(Schema.ItemType ItemType, String ID);

        public Item Item(String ItemTypeName, String ID)
        {
            Schema.ItemType ItemType = this.Schema.ItemType(ItemTypeName);
            return this.ReadItem(ItemType, ID);
        }

        public abstract IEnumerable<Item> ReadItems(Schema.ItemType ItemType, Query.Condition Condition);

        public IEnumerable<Item> ReadItems(String ItemTypeName, Query.Condition Condition)
        {
            Schema.ItemType ItemType = this.Schema.ItemType(ItemTypeName);
            return this.ReadItems(ItemType, Condition);
        }

        public abstract IEnumerable<File> ReadFiles(Schema.FileType FileType);

        public IEnumerable<File> ReadFiles(String FileTypeName)
        {
            Schema.FileType FileType = this.Schema.FileType(FileTypeName);
            return this.ReadFiles(FileType);
        }

        public abstract File ReadFile(Schema.FileType FileType, String ID);

        public File File(String FileTypeName, String ID)
        {
            Schema.FileType FileType = this.Schema.FileType(FileTypeName);
            return this.ReadFile(FileType, ID);
        }

        public abstract IEnumerable<File> ReadFiles(Schema.FileType FileType, Query.Condition Condition);

        public IEnumerable<File> ReadFiles(String FileTypeName, Query.Condition Condition)
        {
            Schema.FileType FileType = this.Schema.FileType(FileTypeName);
            return this.ReadFiles(FileType, Condition);
        }

        public abstract IEnumerable<Relationship> ReadRelationships(Schema.RelationshipType RelationshipType, Item Source);

        public IEnumerable<Relationship> ReadRelationships(String RelationshipTypeName, Item Source)
        {
            Schema.RelationshipType RelationshipType = Source.ItemType.RelationshipType(RelationshipTypeName);
            return this.ReadRelationships(RelationshipType, Source);
        }

        public abstract Relationship ReadRelationship(Schema.RelationshipType RelationshipType, Item Source, String ID);

        public Relationship ReadRelationship(String RelationshipTypeName, Item Source, String ID)
        {
            Schema.RelationshipType RelationshipType = Source.ItemType.RelationshipType(RelationshipTypeName);
            return this.ReadRelationship(RelationshipType, Source, ID);
        }

        public abstract IEnumerable<Relationship> ReadRelationships(Schema.RelationshipType RelationshipType, Item Source, Query.Condition Condition);

        public IEnumerable<Relationship> ReadRelationships(String RelationshipTypeName, Item Source, Query.Condition Condition)
        {
            Schema.RelationshipType RelationshipType = Source.ItemType.RelationshipType(RelationshipTypeName);
            return this.ReadRelationships(RelationshipType, Source, Condition);
        }

        public void Update(Item Item, Transaction Transaction)
        {
            Transaction.Add(Item);
            Item.Update();
        }

        public void Delete(Item Item, Transaction Transaction)
        {
            Transaction.Add(Item);
            Item.Delete();
        }

        public void Dispose()
        {
            this.Close();
        }

        public Session(Schema.Session Schema, String Name, Log Log)
        {
            this.Cache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
            this.Schema = Schema;
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Integrator.Connection.Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, this.ParameterNames);
        }

        public Session(Schema.Session Schema, String Name, String Token, Log Log)
        {
            this.Cache = new Dictionary<Schema.ItemType, Dictionary<String, Item>>();
            this.Schema = Schema;
            this.Name = Name;
            this.Log = Log;
            this.Parameters = new Parameters(this.Name, System.Security.Cryptography.DataProtectionScope.LocalMachine, Token);
        }
    }
}
