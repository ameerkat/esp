using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace Tweet
{
    class TwitterEater
    {
        public int count;
        private Queue<String> unparsedTweets;
        private String twitterStreamUrl, twitterUsername, twitterPassword;
        private Thread tweetReader;

        public TwitterEater(String twitterStreamUrl, String twitterUsername, String twitterPassword)
        {
            this.count = 0;

            this.twitterStreamUrl = twitterStreamUrl;
            this.twitterUsername = twitterUsername;
            this.twitterPassword = twitterPassword;
            unparsedTweets = new Queue<String>();
        }

        private void DoRead()
        {
            WebRequest twitterRequest;
            WebResponse twitterResponse;
            StreamReader twitterStream;

            twitterRequest = (HttpWebRequest)WebRequest.Create(twitterStreamUrl);
            twitterRequest.Credentials = new NetworkCredential(twitterUsername, twitterPassword);
            twitterRequest.Timeout = -1;
            twitterResponse = (HttpWebResponse)twitterRequest.GetResponse();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            twitterStream = new StreamReader(twitterResponse.GetResponseStream(), encode);

            string line;
            while ((line = twitterStream.ReadLine()) != null)
            {
                //Each line from the stream is an unparsed tweet
                try
                {
                    unparsedTweets.Enqueue(line);
                    count++;
                }
                catch (Exception e)
                {
                    //Catch all for now
                    throw new Exception("Reading failed at line: " + line);
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
            catch (Exception e)
            {
                //Catch all for now
                Console.Out.WriteLine(e.Message);
            }
            
        }

        public void Stop()
        {
            tweetReader.Abort();
        }

        public String ReadLine()
        {
            lock(unparsedTweets)
            {
                String result = null;
                try
                {
                    if (unparsedTweets.Count != 0)
                    {
                        result = unparsedTweets.Dequeue();
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.Out.WriteLine("Empty queue");
                }
                return result;
            }
        }
    }
}
