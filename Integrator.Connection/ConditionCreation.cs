using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
{
    public class Conditions
    {
        public static Connection.Conditions.ID ID(String Value)
        {
            return new Connection.Conditions.ID(Value);
        }

        public static Connection.Conditions.Property Eq(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.eq, Value);
        }

        public static Connection.Conditions.Property Ge(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.ge, Value);
        }

        public static Connection.Conditions.Property Gt(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.gt, Value);
        }

        public static Connection.Conditions.Property Le(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.le, Value);
        }

        public static Connection.Conditions.Property Lt(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.lt, Value);
        }

        public static Connection.Conditions.Property Ne(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.ne, Value);
        }

        public static Connection.Conditions.Property Like(String Name, Object Value)
        {
            return new Connection.Conditions.Property(Name, Connection.Conditions.Operators.like, Value);
        }

        public static Connection.Conditions.And And(Connection.Condition Left, Connection.Condition Right)
        {
            return new Connection.Conditions.And(Left, Right);
        }

        public static Connection.Conditions.Or Or(Connection.Condition Left, Connection.Condition Right)
        {
            return new Connection.Conditions.Or(Left, Right);
        }
    }
}
