﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfer.Models.Models
{
    public class ColumnData
    {
        public string ColumnName { get; set; }
        public int Idx { get; set; }


        public string Comparison{ get; set; }

        public string Value { get; set; }
        public string Value2 { get; set; }
    }
}
