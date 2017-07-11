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

        IItem Create(String Name);

        IFile Create(Schema.FileType FileType, String Filename);

        IFile Create(String Name, String Filename);

        IEnumerable<IItem> Index(Schema.ItemType ItemType);

        IEnumerable<IItem> Index(String Name);

        IEnumerable<IItem> Query(Schema.ItemType ItemType, Condition Condition);

        IEnumerable<IItem> Query(String Name, Condition Condition);

        void Close();
    }
}
