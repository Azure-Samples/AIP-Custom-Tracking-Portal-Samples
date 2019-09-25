// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace findmydocs
{
    public class AIPFile
    {


        public class Rootobject
        {
            public Result[] results { get; set; }
            public object render { get; set; }
            public object statistics { get; set; }
            public Table[] tables { get; set; }
        }

        public class Result
        {
            public string FileName { get; set; }
            public string Activity_s { get; set; }
            public string LabelName_s { get; set; }
            public DateTime TimeGenerated { get; set; }
            public string Protected_b { get; set; }
            public string MachineName_s { get; set; }
        }

        public class Table
        {
            public string name { get; set; }
            public Column[] columns { get; set; }
            public object[][] rows { get; set; }
        }

        public class Column
        {
            public string name { get; set; }
            public string type { get; set; }
        }


    }
}