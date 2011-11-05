using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweet.Model
{
    class Tweet
    {
        public ulong id { get; set; }
        public ulong user_id { get; set; }
        public String text { get; set; }
        public String created_at { get; set; }
        public Place place { get; set; }
        public Geo geo { get; set; }
        public override string ToString()
        {
            return "\n[Tweet]\nid : " + id + "\nuser_id : " + user_id + "\ntext : " + text + "\ndate : " + created_at + "\nplace : " + place + "\n[/Tweet]";
        }
    }
}
