using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace Tweet
{
    class Program
    {
        static int count = 0;
        static void Main(string[] args)
        {
            TweetProducer twitterEater = new TweetProducer(Options.twitterStreamUrl, Options.twitterUsername, Options.twitterPassword);
            twitterEater.Start();

            for (int i = 0; i < Options.numberThreads; i++)
            {
                TweetConsumer consumer = new TweetConsumer(twitterEater, i);
                consumer.Start();
            }
        }

    }
}
