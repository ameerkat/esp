﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class BoundingBox
    {
        public String type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
        public override string ToString()
        {
            return "type : " + type + "\ncoordinates: " + coordinates;
        }
    }
}
