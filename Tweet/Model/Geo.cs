using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class Geo
    {
        public String type { get; set; }
        public List<Double> coordinates { get; set; }
        public override string ToString(){
            return "\n[Geo]\ntype : " + type + "\ncoordinates: " + coordinates + "\n[/Geo]";
        }
    }
}
