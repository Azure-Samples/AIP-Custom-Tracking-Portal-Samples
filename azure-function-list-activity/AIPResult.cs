// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackAIPFiles
{
    class AIPResult
    {


        public class Rootobject
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
