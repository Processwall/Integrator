﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integrator.Connection.Aras.Properties
{
    public class MD5 : Property, Connection.Properties.IMD5
    {

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if ((value == null) || (value is System.String))
                {
                    base.Object = value;
                }
                else
                {
                    throw new Exceptions.ArgumentException("Object value must be System.String");
                }
            }
        }

        public System.String Value
        {
            get
            {
                return (System.String)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        internal override System.String DBValue
        {
            get
            {
                return this.Value;

            }
            set
            {
                this.Value = value;
            }
        }

        internal MD5(PropertyTypes.MD5 PropertyType)
            : base(PropertyType)
        {
        }
    }
}
