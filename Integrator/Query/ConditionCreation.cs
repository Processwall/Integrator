using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator
{
    public class Conditions
    {
        public static Query.Conditions.ID ID(String Value)
        {
            return new Query.Conditions.ID(Value);
        }

        public static Query.Conditions.Property Eq(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.eq, Value);
        }

        public static Query.Conditions.Property Ge(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.ge, Value);
        }

        public static Query.Conditions.Property Gt(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.gt, Value);
        }

        public static Query.Conditions.Property Le(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.le, Value);
        }

        public static Query.Conditions.Property Lt(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.lt, Value);
        }

        public static Query.Conditions.Property Ne(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.ne, Value);
        }

        public static Query.Conditions.Property Like(String Name, Object Value)
        {
            return new Query.Conditions.Property(Name, Query.Conditions.Operators.like, Value);
        }

        public static Query.Conditions.And And(Query.Condition Left, Query.Condition Right)
        {
            return new Query.Conditions.And(Left, Right);
        }

        public static Query.Conditions.Or Or(Query.Condition Left, Query.Condition Right)
        {
            return new Query.Conditions.Or(Left, Right);
        }
    }
}
