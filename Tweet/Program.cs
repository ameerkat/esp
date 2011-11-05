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
            TwitterEater twitterEater = new TwitterEater(Options.twitterStreamUrl, Options.twitterUsername, Options.twitterPassword);
            twitterEater.Start();


            int numThreads = Options.numberThreads;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(Consume));
                object[] parameters = new object[] { twitterEater, i };
                thread.Start(parameters);
            }
        }

        private static void Consume(object parameters)
        {
            object[] parameterArray = (object[])parameters;
            IConsumer eater = (IConsumer)parameterArray[0];
            int threadNumber = (int)parameterArray[1];
            String parseMe;
            while (true)
            {
                if ((parseMe = eater.ReadLine()) != null)
                {
                    Model.Status status = eater.deserialize(parseMe);
                    Console.Out.WriteLine("Thread " + threadNumber + " : \n" + status);
                    count++;
                    Console.Out.WriteLine("Deserialized: " + count + ", Read: " + eater.getCount());
                    //Console.Out.WriteLine("Thread " + threadNumber + ": \t" + parseMe);
                }
                else
                {
                    Console.Out.WriteLine("Thread " + threadNumber + ": \tNothing to parse, waiting...");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
