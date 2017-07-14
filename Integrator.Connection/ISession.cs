using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection
{
    public interface ISession : IEquatable<ISession>
    {
        String Name { get; }

        String Token(String Group, String Username, String Password);

        Parameters Parameters { get; }

        void Login(String Token);

        Schema.Session Schema { get; }

        IItem Create(Schema.ItemType ItemType);

        IItem Create(String ItemTypeName);

        IFile Create(Schema.FileType FileType, String Filename);

        IFile Create(String FileTypeName, String Filename);

        IEnumerable<IItem> Index(Schema.ItemType ItemType);

        IEnumerable<IItem> Index(String ItemTypeName);

        IEnumerable<IItem> Query(Schema.ItemType ItemType, Condition Condition);

        IEnumerable<IItem> Query(String ItemTypeName, Condition Condition);

        IItem Get(String ID);

        void Close();
    }
}
