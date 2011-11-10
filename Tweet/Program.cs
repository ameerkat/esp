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
            /* add in another thread that checks the database of tweets for events
             * procedure:
             * 1) cluster tweets into events
             * 2) compare to old event 'clusters', if a cluster has a new event then up the lifetime of all of them
             * 3) clear out expired tweets
             * 4) if we detect an event then push to an events database, perhaps trigger a call back that
                does push notifications to phones if in X miles of event. (eventually we will put in component
                that will predict affected areas), either way we need to call some function at this point.
             * 5) an events API that will be made accessible to the client so that they can get event information,
                as well as the cluster of tweets that support the event.
             */
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
