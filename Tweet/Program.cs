using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace Tweet
{
    class Program
    {

        static void Main(string[] args)
        {
            TwitterEater twitterEater = new TwitterEater(Options.twitterStreamUrl, Options.twitterUsername, Options.twitterPassword);
            twitterEater.Start();


            int numThreads = 4;
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
            TwitterEater twitterEater = (TwitterEater)parameterArray[0];
            int threadNumber = (int)parameterArray[1];
            String parseMe;
            while (true)
            {
                if ((parseMe = twitterEater.ReadLine()) != null)
                {
                    //Parse this
                    Console.Out.WriteLine("Thread " + threadNumber + ": \t" + parseMe);
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
