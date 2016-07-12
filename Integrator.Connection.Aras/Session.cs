using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IOM = Aras.IOM;
using System.Xml;

namespace Integrator.Connection.Aras
{
    public class Session : ISession
    {
        public String URL { get; private set; }

        public String Database { get; private set; }

        public String Username { get; private set; }

        public String Password { get; private set; }

        private DirectoryInfo _workspace;
        internal DirectoryInfo WorkSpace
        {
            get
            {
                if (this._workspace == null)
                {
                    this._workspace = new DirectoryInfo(Path.GetTempPath() + "\\Integrator\\Aras");

                    if (!this._workspace.Exists)
                    {
                        this._workspace.Create();
                    }
                }

                return this._workspace;
            }
        }

        public bool Equals(ISession other)
        {
            if (other != null)
            {
                if (other is Session)
                {
                    return this.URL.Equals(((Session)other).URL) && this.Database.Equals(((Session)other).Database) && this.Username.Equals(((Session)other).Username);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private IOM.Innovator _innovator;
        internal IOM.Innovator Innovator
        {
            get
            {
                if (this._innovator == null)
                {
                    IOM.HttpServerConnection connection = IOM.IomFactory.CreateHttpServerConnection(this.URL, this.Database, this.Username, this.Password);
                    IOM.Item user = connection.Login();

                    if (!user.isError())
                    {
                        this._innovator = IOM.IomFactory.CreateInnovator(connection);
                    }
                    else
                    {
                        throw new Exceptions.LoginException(user.getErrorString());
                    }
                }

                return this._innovator;
            }
        }

        private void ProcessClassStructure(ItemType Base, XmlNode Node)
        {
            if (Node != null)
            {
                foreach (XmlNode childnode in Node.SelectNodes("class"))
                {
                    String id = childnode.Attributes["id"].Value;
                    String name = childnode.Attributes["name"].Value;

                    if (Base is RelationshipType)
                    {
                        RelationshipType relationshiptype = new RelationshipType(this, (RelationshipType)Base, id, Base.Name + "." + name, Base.CanVersion);
                        this._itemTypeCache[relationshiptype.ID] = relationshiptype;
                        this.ProcessClassStructure(relationshiptype, childnode);
                    }
                    else if (Base is FileType)
                    {
                        FileType filetype = new FileType(this, (FileType)Base, id, Base.Name + "." + name, Base.CanVersion);
                        this._itemTypeCache[filetype.ID] = filetype;
                        this.ProcessClassStructure(filetype, childnode);
                    }
                    else
                    {
                        ItemType itemtype = new ItemType(this, Base, id, Base.Name + "." + name, Base.CanVersion);
                        this._itemTypeCache[itemtype.ID] = itemtype;
                        this.ProcessClassStructure(itemtype, childnode);
                    }
                }
            }
        }

        private IEnumerable<ItemType> SubTypes(ItemType ItemType)
        {
            List<ItemType> ret = new List<ItemType>();

            foreach(ItemType subitemtype in this._itemTypeCache.Values)
            {
                if ((subitemtype.Parent) != null && subitemtype.Parent.Equals(ItemType))
                {
                    ret.Add(subitemtype);

                    foreach(ItemType subsubitemtype in this.SubTypes(subitemtype))
                    {
                        ret.Add(subsubitemtype);
                    }
                }
            }

            return ret;
        }

        private Dictionary<String, ItemType> _itemTypeCache;
        private Dictionary<String, ItemType> ItemTypeCache
        {
            get
            {
                if (this._itemTypeCache == null)
                {
                    this._itemTypeCache = new Dictionary<String, ItemType>();

                    // Read ItemTypes
                    IOM.Item iomitemtypes = this.Innovator.newItem("ItemType", "get");
                    iomitemtypes.setAttribute("select", "id,name,is_relationship,is_versionable,class_structure");
                    iomitemtypes = iomitemtypes.apply();

                    if (!iomitemtypes.isError())
                    {
                        for (int i = 0; i < iomitemtypes.getItemCount(); i++)
                        {
                            IOM.Item iomitemtype = iomitemtypes.getItemByIndex(i);

                            // Process ClassStructure
                            XmlNode classnode = null;

                            if (!String.IsNullOrEmpty(iomitemtype.getProperty("class_structure")))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(iomitemtype.getProperty("class_structure"));
                                classnode = doc.SelectSingleNode("class");
                            }

                            if (iomitemtype.getProperty("is_relationship", "0").Equals("1"))
                            {
                                RelationshipType relationshiptype = new RelationshipType(this, null, iomitemtype.getID(), iomitemtype.getProperty("name"), iomitemtype.getProperty("is_versionable", "0").Equals("1"));
                                this._itemTypeCache[relationshiptype.ID] = relationshiptype;
                                this.ProcessClassStructure(relationshiptype, classnode);
                            }
                            else
                            {
                                if (iomitemtype.getProperty("name").Equals("File"))
                                {
                                    FileType filetype = new FileType(this, null, iomitemtype.getID(), iomitemtype.getProperty("name"), iomitemtype.getProperty("is_versionable", "0").Equals("1"));
                                    this._itemTypeCache[filetype.ID] = filetype;
                                    this.ProcessClassStructure(filetype, classnode);
                                }
                                else
                                {
                                    ItemType itemtype = new ItemType(this, null, iomitemtype.getID(), iomitemtype.getProperty("name"), iomitemtype.getProperty("is_versionable", "0").Equals("1"));
                                    this._itemTypeCache[itemtype.ID] = itemtype;
                                    this.ProcessClassStructure(itemtype, classnode);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomitemtypes.getErrorString());
                    }

                    // Read RelationshipTypes
                    IOM.Item iomrelationshiptypes = this.Innovator.newItem("RelationshipType", "get");
                    iomrelationshiptypes.setAttribute("select", "source_id,related_id,relationship_id");
                    iomrelationshiptypes = iomrelationshiptypes.apply();

                    if (!iomrelationshiptypes.isError())
                    {
                        for (int i = 0; i < iomrelationshiptypes.getItemCount(); i++)
                        {
                            IOM.Item iomrelationshiptype = iomrelationshiptypes.getItemByIndex(i);
                            
                            // Store Source
                            String source_id = iomrelationshiptype.getProperty("source_id", null);

                            if (!String.IsNullOrEmpty(source_id))
                            {
                                ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Source = this._itemTypeCache[source_id];
                            }

                            // Store Related
                            String related_id = iomrelationshiptype.getProperty("related_id", null);

                            if (!String.IsNullOrEmpty(related_id))
                            {
                                ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Related = this._itemTypeCache[related_id];
                            }

                            // Process SubTypes
                            foreach (RelationshipType subreltype in this.SubTypes(this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]))
                            {
                                subreltype.Source = ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Source;
                                subreltype.Related = ((RelationshipType)this._itemTypeCache[iomrelationshiptype.getProperty("relationship_id")]).Related;
                            }
                        }
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomitemtypes.getErrorString());
                    }
                }

                return this._itemTypeCache;
            }
        }

        private List<ItemType> _itemTypes;
        public IEnumerable<IItemType> ItemTypes
        {
            get
            {
                if (this._itemTypes == null)
                {
                    this._itemTypes = new List<ItemType>();

                    foreach (ItemType itemtype in this.ItemTypeCache.Values)
                    {
                        if (!(itemtype is RelationshipType))
                        {
                            this._itemTypes.Add(itemtype);
                        }
                    }
                }

                return this._itemTypes;
            }
        }

        private List<FileType> _fileTypes;
        public IEnumerable<IFileType> FileTypes
        {
            get
            {
                if (this._fileTypes == null)
                {
                    this._fileTypes = new List<FileType>();

                    foreach (ItemType itemtype in this.ItemTypeCache.Values)
                    {
                        if (itemtype is FileType)
                        {
                            this._fileTypes.Add((FileType)itemtype);
                        }
                    }
                }

                return this._fileTypes;
            }
        }

        public IFileType FileType(String Name)
        {
            foreach (FileType itemtype in this.FileTypes)
            {
                if (itemtype.Name.Equals(Name))
                {
                    return itemtype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid FileType Name: " + Name);
        }

        internal ItemType ItemTypeByID(String ID)
        {
            if (this.ItemTypeCache.ContainsKey(ID))
            {
                return this.ItemTypeCache[ID];
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType ID: " + ID);
            }
        }

        public IItemType ItemType(String Name)
        {
            foreach (ItemType itemtype in this.ItemTypeCache.Values)
            {
                if (itemtype.Name.Equals(Name))
                {
                    return itemtype;
                }
            }

            throw new Exceptions.ArgumentException("Invalid ItemType Name: " + Name);
        }

        private Dictionary<ItemType, Dictionary<String, Item>> ItemCache;

        internal Item Create(ItemType ItemType, String ID, State Status)
        {
            if (!this.ItemCache.ContainsKey(ItemType))
            {
                this.ItemCache[ItemType] = new Dictionary<String,Item>();
            }

            if (this.ItemCache[ItemType].ContainsKey(ID))
            {
                return this.ItemCache[ItemType][ID];
            }
            else
            {
                if (ItemType is RelationshipType)
                {
                    IOM.Item iomsource = this.Innovator.newItem(((ItemType)ItemType).DBName, "get");
                    iomsource.setID(ID);
                    iomsource.setAttribute("select", "id,source_id,related_id");
                    iomsource = iomsource.apply();

                    if (!iomsource.isError())
                    {
                        return this.Create((RelationshipType)ItemType, ID, State.Stored, iomsource.getProperty("source_id"), iomsource.getProperty("related_id"));
                    }
                    else
                    {
                        throw new Exceptions.ReadException(iomsource.getErrorString());
                    }
                }
                else if (ItemType is FileType)
                {
                    File file = new File((FileType)ItemType, ID, Status);
                    this.ItemCache[ItemType][ID] = file;
                    return file;
                }
                else
                {
                    Item item = new Item(ItemType, ID, Status);
                    this.ItemCache[ItemType][ID] = item;
                    return item;
                }
            }
        }

        internal Relationship Create(RelationshipType RelationshipType, String ID, State Status, String SourceID, String RelatedID)
        {
            if (!this.ItemCache.ContainsKey(RelationshipType))
            {
                this.ItemCache[RelationshipType] = new Dictionary<String, Item>();
            }

            if (this.ItemCache[RelationshipType].ContainsKey(ID))
            {
                return (Relationship)this.ItemCache[RelationshipType][ID];
            }
            else
            {
                Item source = null;
                Item related = null;

                if (!System.String.IsNullOrEmpty(SourceID))
                {
                    if (RelationshipType.Source is RelationshipType)
                    {
                        IOM.Item iomsource = this.Innovator.newItem(((ItemType)RelationshipType.Source).DBName, "get");
                        iomsource.setID(SourceID);
                        iomsource.setAttribute("select", "id,source_id,related_id");
                        iomsource = iomsource.apply();

                        if (!iomsource.isError())
                        {
                            source = this.Create((RelationshipType)RelationshipType.Source, SourceID, State.Stored, iomsource.getProperty("source_id"), iomsource.getProperty("related_id"));
                        }
                        else
                        {
                            throw new Exceptions.ReadException(iomsource.getErrorString());
                        }
                    }
                    else
                    {
                        source = this.Create((ItemType)RelationshipType.Source, SourceID, Status);
                    }
                }

                if (RelationshipType.Related != null)
                {
                    if (!System.String.IsNullOrEmpty(RelatedID))
                    {
                        if (RelationshipType.Related is RelationshipType)
                        {
                            IOM.Item iomrelated = this.Innovator.newItem(((ItemType)RelationshipType.Related).DBName, "get");
                            iomrelated.setID(RelatedID);
                            iomrelated.setAttribute("select", "id,source_id,related_id");
                            iomrelated = iomrelated.apply();

                            if (!iomrelated.isError())
                            {
                                related = this.Create((RelationshipType)RelationshipType.Related, RelatedID, State.Stored, iomrelated.getProperty("source_id"), iomrelated.getProperty("related_id"));
                            }
                            else
                            {
                                throw new Exceptions.ReadException(iomrelated.getErrorString());
                            }
                        }
                        else
                        {
                            related = this.Create((ItemType)RelationshipType.Related, RelatedID, State.Stored);
                        }
                    }
                }

                Relationship relationship = new Relationship(RelationshipType, ID, Status, source, related);
                this.ItemCache[RelationshipType][ID] = relationship;
                return relationship;
            }
        }

        public IItem Create(IItemType ItemType)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                String ID = this.Innovator.getNewID();
                Item item = this.Create((ItemType)ItemType, ID, State.Created);
                return item;
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public IItem Create(String Name)
        {
            return this.Create(this.ItemType(Name));
        }

        public IFile Create(IFileType FileType, String Filename)
        {
            if (FileType is FileType && ((FileType)FileType).Session.Equals(this))
            {
                String ID = this.Innovator.getNewID();
                File item = (File)this.Create((FileType)FileType, ID, State.Created);
                item.Filename = Filename;
                return item;
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid FileType");
            }
        }

        public IFile Create(String Name, String Filename)
        {
            return this.Create(this.FileType(Name), Filename);
        }

        public IEnumerable<IItem> Index(IItemType ItemType)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                IOM.Item iomitems = this.Innovator.newItem(((ItemType)ItemType).DBName, "get");
                iomitems.setAttribute("select", "id,classification");

                if (((ItemType)ItemType).DBClassification != null)
                {
                    iomitems.setProperty("classification", ((ItemType)ItemType).DBClassification);
                }

                iomitems = iomitems.apply();

                if (!iomitems.isError())
                {
                    List<Item> items = new List<Item>();

                    for (int i = 0; i < iomitems.getItemCount(); i++)
                    {
                        IOM.Item iomitem = iomitems.getItemByIndex(i);
                        items.Add(this.Create(((ItemType)ItemType).SubTypeFromClassification(iomitem.getProperty("classification")), iomitem.getID(), State.Stored));
                    }

                    return items;
                }
                else
                {
                    String errormessage = iomitems.getErrorString();

                    if (errormessage.Equals("No items of type " + ((ItemType)ItemType).DBName + " found."))
                    {
                        return new List<Item>();
                    }
                    else
                    {
                        throw new Exceptions.ReadException(errormessage);
                    }
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public IEnumerable<IItem> Index(String Name)
        {
            return this.Index(this.ItemType(Name));
        }

        private static System.String OperatorString(Conditions.Operators Operator)
        {
            switch (Operator)
            {
                case Conditions.Operators.eq:
                    return "=";
                case Conditions.Operators.ne:
                    return "<>";
                case Conditions.Operators.gt:
                    return ">";
                case Conditions.Operators.lt:
                    return "<";
                case Conditions.Operators.le:
                    return "<=";
                case Conditions.Operators.ge:
                    return ">=";
                case Conditions.Operators.like:
                    return "like";
                default:
                    throw new NotImplementedException("Property Condition Operator not implemented");
            }
        }

        internal String Where(IItemType ItemType, Condition Condition)
        {
            String where = null;

            switch(Condition.GetType().Name)
            {
                case "Property":

                    if (((Conditions.Property)Condition).Name == "id")
                    {
                        if (((Conditions.Property)Condition).Value == null)
                        {
                            where = "(" + ((ItemType)ItemType).TableName + ".[id] is null)";
                        }
                        else
                        {
                            return "(" + ((ItemType)ItemType).TableName + ".[id]" + OperatorString(((Conditions.Property)Condition).Operator) + "'" + ((Conditions.Property)Condition).Value.ToString() + "')";
                        }
                    }
                    else
                    {
                        PropertyType proptype = (PropertyType)ItemType.PropertyType(((Conditions.Property)Condition).Name);

                        switch (proptype.GetType().Name)
                        {
                            case "String":

                                if (((Conditions.Property)Condition).Value == null)
                                {
                                    return "(" + proptype.ColumnName + " is null)";
                                }
                                else
                                {
                                    return "(" + proptype.ColumnName + OperatorString(((Conditions.Property)Condition).Operator) + "'" + ((Conditions.Property)Condition).Value.ToString().Replace('*', '%') + "')";
                                }

                            default:
                                throw new Exceptions.ArgumentException("Property Type not implemented: " + proptype.GetType().Name);
                        }
                    }

                    break;

                case "Or":

                    switch (Condition.Children.Count())
                    {
                        case 0:
                            throw new Exceptions.ArgumentException("Invalid Or Condition, must have at leat one Child");
                        case 1:
                            where = this.Where(ItemType, Condition.Children.First());

                            break;
                        default:
                            where = "(" + this.Where(ItemType, Condition.Children.First());

                            for (int i = 1; i < Condition.Children.Count(); i++)
                            {
                                where += " or " + this.Where(ItemType, Condition.Children.ElementAt(i));
                            }

                            where += ")";

                            break;
                    }

                    break;

                case "And":

                    switch (Condition.Children.Count())
                    {
                        case 0:
                            throw new Exceptions.ArgumentException("Invalid And Condition, must have at leat one Child");
                        case 1:
                            where = this.Where(ItemType, Condition.Children.First());

                            break;
                        default:
                            where = "(" + this.Where(ItemType, Condition.Children.First());

                            for (int i = 1; i < Condition.Children.Count(); i++)
                            {
                                where += " and " + this.Where(ItemType, Condition.Children.ElementAt(i));
                            }

                            where += ")";

                            break;
                    }

                    break;

                default:
                    throw new Exceptions.ArgumentException("Condition not implemented: " + Condition.GetType().Name);
            }

            return where;
        }

        public IEnumerable<IItem> Query(String Name, Condition Condition)
        {
            return this.Query(this.ItemType(Name), Condition);
        }

        public IEnumerable<IItem> Query(IItemType ItemType, Condition Condition)
        {
            if (ItemType is ItemType && ((ItemType)ItemType).Session.Equals(this))
            {
                IOM.Item iomitems = this.Innovator.newItem(((ItemType)ItemType).DBName, "get");
                iomitems.setAttribute("select", "id,classification");

                if (((ItemType)ItemType).DBClassification != null)
                {
                    iomitems.setProperty("classification", ((ItemType)ItemType).DBClassification);
                }

                iomitems.setAttribute("where", this.Where(ItemType, Condition));
                iomitems = iomitems.apply();

                if (!iomitems.isError())
                {
                    List<Item> items = new List<Item>();

                    for (int i = 0; i < iomitems.getItemCount(); i++)
                    {
                        IOM.Item iomitem = iomitems.getItemByIndex(i);
                        items.Add(this.Create(((ItemType)ItemType).SubTypeFromClassification(iomitem.getProperty("classification")), iomitem.getID(), State.Stored));
                    }

                    return items;
                }
                else
                {
                    String errormessage = iomitems.getErrorString();

                    if (errormessage.Equals("No items of type " + ((ItemType)ItemType).DBName + " found."))
                    {
                        return new List<Item>();
                    }
                    else
                    {
                        throw new Exceptions.ReadException(errormessage);
                    }
                }
            }
            else
            {
                throw new Exceptions.ArgumentException("Invalid ItemType");
            }
        }

        public void Close()
        {

        }

        public Session(String URL, String Database, String Username, String Password)
        {
            this.URL = URL;
            this.Database = Database;
            this.Username = Username;
            this.Password = Password;
            this.ItemCache = new Dictionary<ItemType,Dictionary<String,Item>>();
        }
    }
}
