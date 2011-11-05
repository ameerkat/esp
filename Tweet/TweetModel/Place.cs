using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class Place
    {
        public BoundingBox bounding_box { get; set; }
        public override string ToString()
        {
            return "\n[Place]\nbounding_box : " + bounding_box + "\n[/Place]";
        }
    }
}
