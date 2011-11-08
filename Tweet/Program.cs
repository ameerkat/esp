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
        static void Main(string[] args)
        {
            Dictionary<string, string> twitterOptions = new Dictionary<string, string>();
            twitterOptions.Add("streamUrl", Options.twitterStreamUrl);
            twitterOptions.Add("username", Options.twitterUsername);
            twitterOptions.Add("password", Options.twitterPassword);
            TweetProducer twitterProducer = new TweetProducer(twitterOptions);
            twitterProducer.AddKeyword("fire");
            twitterProducer.Start();
            Console.Out.WriteLine("stream url : " + twitterProducer.GetCurrentUrl());
            for (int i = 0; i < Options.numberThreads; i++)
            {
                TweetConsumer consumer = new TweetConsumer(twitterProducer, i);
                consumer.Start();
            }
        }

    }
}
