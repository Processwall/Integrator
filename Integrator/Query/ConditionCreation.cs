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
