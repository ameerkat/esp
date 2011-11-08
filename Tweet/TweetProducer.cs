﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace Tweet
{
    //Reads tweets from twitter streaming API as strings
    class TweetProducer : IProducer
    {
        private int count;
        private Queue<String> unparsedTweets;
        private String twitterStreamUrl, twitterUsername, twitterPassword;
        private Thread tweetReader;
        // required values passed as options to the constructor in the options dictionary
        public static string[] requiredValues = { "username", "password", "streamUrl" };

        public TweetProducer(String twitterStreamUrl, String twitterUsername, String twitterPassword)
        {
            this.count = 0;
            this.twitterStreamUrl = twitterStreamUrl;
            this.twitterUsername = twitterUsername;
            this.twitterPassword = twitterPassword;
            this.unparsedTweets = new Queue<String>();
        }

        // Same as the plain constructor except takes a dictionary of options instead
        public TweetProducer(Dictionary<string, string> twitterOptions)
        {
            this.count = 0;
            foreach (string s in requiredValues){
                if (!twitterOptions.ContainsKey(s)){
                    System.ArgumentException argEx = new System.ArgumentException("Required options key missing : " + s);
                    throw argEx;
                }
            }
            this.twitterStreamUrl = twitterOptions["streamUrl"];
            this.twitterUsername = twitterOptions["username"];
            this.twitterPassword = twitterOptions["password"];
            this.unparsedTweets = new Queue<String>();
        }

        public int Count()
        {
            return this.count;
        }

        private void DoRead()
        {
            WebRequest twitterRequest = (HttpWebRequest)WebRequest.Create(twitterStreamUrl);
            twitterRequest.Credentials = new NetworkCredential(twitterUsername, twitterPassword);
            twitterRequest.Timeout = -1;
            WebResponse twitterResponse = (HttpWebResponse)twitterRequest.GetResponse();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            //Automatically releases handle on stream reader when necessary
            using (StreamReader twitterStream = new StreamReader(twitterResponse.GetResponseStream(), encode))
            {
                try
                {
                    string line;
                    while ((line = twitterStream.ReadLine()) != null)
                    {
                        //Each line from the stream is an unparsed tweet
                        this.unparsedTweets.Enqueue(line);
                        this.count++;
                    }
                }
                catch (OutOfMemoryException e)
                {
                    //Print out exception message and return for now, consider actual handling of this exception
                    Console.Out.WriteLine(e.Message);
                    return;
                }
                catch (IOException e)
                {
                    //Print out exception message and return for now, consider actual handling of this exception
                    Console.Out.WriteLine(e.Message);
                    return;
                }
            }
        }

        public void Start()
        {
            try
            {
                tweetReader = new Thread(new ThreadStart(DoRead));
                tweetReader.Start();
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
                tweetReader.Abort();
            }
            catch (ThreadStateException e)
            {
                //Print out exception message and return for now, consider actual handling of this exception
                Console.Out.WriteLine(e.Message);
                return;
            }
        }

        public String ReadLine()
        {
            lock(this.unparsedTweets)
            {
                String result = null;
                if (this.unparsedTweets.Count != 0)
                {
                    result = this.unparsedTweets.Dequeue();
                }
                return result;
            }
        }
    }
}
