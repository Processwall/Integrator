﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integrator.Sync.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Session session = new Session(new FileInfo("D:\\dev\\LM\\Mapping\\Sync.xml"));

            Action action = session.Action("Assembly from Teamcenter to Aras");
            Integrator.Parameters parameters = action.CreateParameters();
            parameters.Parameter("Number").Value = "1746798-3";
            action.Execute(parameters);
        }
    }
}
