using System;
using System.Web.Script.Serialization;
using Tweet.Model;
using System.Threading;
using System.Collections.Generic;

namespace Tweet
{
    class TweetConsumer : IConsumer
    {
        private TweetProducer producer;
        private int count;
        private Thread consumer;
        private int threadNumber;

        public TweetConsumer(TweetProducer producer, int threadNumber)
        {
            this.count = 0; 
            this.producer = producer;
            this.threadNumber = threadNumber;
        }

        public void Start()
        {
            try
            {
                this.consumer = new Thread(new ThreadStart(DoConsume));
                this.consumer.Start();
            }
            catch (OutOfMemoryException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
            catch (ThreadStateException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
        }

        public void Stop()
        {
            try
            {
                this.consumer.Abort();
            }
            catch (ThreadStateException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
        }

        private void DoConsume()
        {
            String parseMe;
            while (true)
            {
                if ((parseMe = this.producer.ReadLine()) != null)
                {
                    Status status = this.Deserialize(parseMe);
                    this.count++;
                    Console.Out.WriteLine("Thread " + this.threadNumber + ":\n" + status.ToString());
                }
                else
                {
                    //No tweets to consume, wait 1 second
                    Console.Out.WriteLine("Thread " + this.threadNumber + ": Nothing to consume, waiting...");
                    Thread.Sleep(1000);
                }
            }
        }

        public int Count()
        {
            return this.count;
        }

        public Status Deserialize(String parseMe)
        {
            // Twitter specific deserialization, except all return Model.Status
            // type objects.
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Status tweet = ser.Deserialize<Status>(parseMe);
            return tweet;
        }
    }
}
