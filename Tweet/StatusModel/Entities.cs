using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class Entities
    {
        public HashTags hashtags { get; set; }
        public override string ToString()
        {
            return "\n[Entities]\nhashtags : " + hashtags + "\n[/Entities]";
        }
    }
}
