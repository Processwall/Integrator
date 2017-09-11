using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface ISession
    {
        String Name { get; set; }

        Parameters Parameters { get; }

        Schema.Session Schema { get; set; }

        Log Log { get; set; }

        void Open();

        void Open(String Token);

        void Close();

        ITransaction BeginTransaction();

        IItem Create(ITransaction Transaction, Schema.ItemType ItemType);

        IItem Create(ITransaction Transaction, String ItemTypeName);

        IFile Create(ITransaction Transaction, Schema.FileType FileType, String Filename);

        IFile Create(ITransaction Transaction, String FileTypeName, String Filename);

        IEnumerable<IItem> Index(Schema.ItemType ItemType);

        IEnumerable<IItem> Index(String ItemTypeName);

        IEnumerable<IItem> Query(Schema.ItemType ItemType, Condition Condition);

        IEnumerable<IItem> Query(String ItemTypeName, Condition Condition);

        IItem Get(String ID);  
    }
}
