using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection.SQLServer
{
    internal class Column
    {
        internal Table Table { get; private set; }

        internal String Name { get; private set; }

        internal String Type { get; private set; }

        internal Boolean IsNullable { get; private set; }

        internal Int32 MaxLength { get; private set; }

        internal Boolean PrimaryIndex { get; private set; }

        internal Boolean Exists { get; private set; }

        private String _sQL;
        internal String SQL
        {
            get
            {
                if (this._sQL == null)
                {
                    this._sQL = "[" + this.Name + "] " + this.Type;

                    if (this.Type == "nvarchar")
                    {
                        this._sQL += "(" + this.MaxLength + ")";
                    }

                    if (!this.IsNullable)
                    {
                        this._sQL += " not null";
                    }

                    if (this.PrimaryIndex)
                    {
                        this._sQL += " primary key";
                    }
                }

                return this._sQL;
            }
        }

        public override string ToString()
        {
            return this.Name + " (" + this.Type + ")";
        }

        internal Column(Table Table, String Name, String Type, Boolean IsNullable, Int32 MaxLength, Boolean PrimaryIndex, Boolean Exists)
        {
            this.Table = Table;
            this.Name = Name;
            this.Type = Type;
            this.IsNullable = IsNullable;
            this.MaxLength = MaxLength;
            this.PrimaryIndex = PrimaryIndex;
            this.Exists = Exists;
        }
    }
}
