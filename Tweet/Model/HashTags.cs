using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class HashTags
    {
        public String text { get; set; }
        public List<ulong> indices { get; set; }
        public override string ToString()
        {
            return "\n[HashTags]\ntext : " + text + "\nindices : " + indices + "\n[/HashTags]";
        }
    }
}
