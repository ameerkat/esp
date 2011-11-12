using System;
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
        // note we build the full URL later, after we set geo options and
        // add keywords to track this is just the base URL
        private String twitterStreamUrl, twitterUsername, twitterPassword;
        private String fullTwitterStreamUrl;
        private Thread tweetReader;

        private List<String> trackKeywords;
        private bool isGeocoded;
        private double[] coordinates;

        // required values passed as options to the constructor in the options dictionary
        public static String[] requiredValues = { "username", "password", "streamUrl" };

        public TweetProducer(String twitterStreamUrl, String twitterUsername, String twitterPassword)
        {
            this.count = 0;
            this.twitterStreamUrl = twitterStreamUrl;
            this.twitterUsername = twitterUsername;
            this.twitterPassword = twitterPassword;
            this.fullTwitterStreamUrl = null;
            this.unparsedTweets = new Queue<String>();
            this.trackKeywords = new List<String>();
            this.isGeocoded = true;
            this.coordinates = new double[4]{-180.0, -90.0, 180.0, 90.0};
        }

        // Same as the plain constructor except takes a dictionary of options instead
        public TweetProducer(Dictionary<String, String> twitterOptions)
        {
            this.count = 0;
            foreach (String s in requiredValues){
                if (!twitterOptions.ContainsKey(s)){
                    System.ArgumentException argEx = new System.ArgumentException("Required options key missing : " + s);
                    throw argEx;
                }
            }
            this.twitterStreamUrl = twitterOptions["streamUrl"];
            this.twitterUsername = twitterOptions["username"];
            this.twitterPassword = twitterOptions["password"];
            this.unparsedTweets = new Queue<String>();
            this.trackKeywords = new List<String>();
            this.fullTwitterStreamUrl = null;
            this.isGeocoded = true;
            this.coordinates = new double[4] { -180.0, -90.0, 180.0, 90.0 };
        }

        /*
         * Keyword Operations
         */
        public void AddKeyword(String kw){
            this.trackKeywords.Add(kw);
        }
        public void RemoveKeyword(String kw){
            this.trackKeywords.Remove(kw);
        }
        public List<String> GetKeywords(){
            return trackKeywords;
        }
        public void SetKeywords(List<String> trackKeywords){
            this.trackKeywords = trackKeywords;
        }

        /*
         * Geo Options
         */
        public bool IsGeocoded(){
            return this.isGeocoded;
        }
        public void SetGeocoded(bool geo){
            this.isGeocoded = geo;
        }
        public void SetCoordinates(double c1, double c2, double c3, double c4){
            this.isGeocoded = true;
            this.coordinates[0] = c1;
            this.coordinates[1] = c2;
            this.coordinates[2] = c3;
            this.coordinates[3] = c4;
        }
        public void SetCoordinates(double[] coords){
            this.isGeocoded = true;
            this.coordinates = coords;
        }
        public double[] GetCoordinates(){
            return this.coordinates;
        }
        public void ClearCoordinates(){
            this.coordinates = new double[4] { -180.0, -90.0, 180.0, 90.0 };
        }

        private String BuildTwitterUrl(){
            bool has_params = false;
            this.fullTwitterStreamUrl = this.twitterStreamUrl;
            /*if (trackKeywords.Count > 0)
            {
                // add track keyword params
                if (has_params) {
                    this.fullTwitterStreamUrl += "&";
                } else {
                    has_params = true;
                    this.fullTwitterStreamUrl += "?";
                }
                this.fullTwitterStreamUrl += "track=" + string.Join(",", trackKeywords.ToArray());
            }*/
            if (this.isGeocoded)
            {
                if (has_params){
                    this.fullTwitterStreamUrl += "&";
                }
                else{
                    has_params = true;
                    this.fullTwitterStreamUrl += "?";
                }
                this.fullTwitterStreamUrl += "locations=" + string.Join(",", coordinates);
            }
            return this.fullTwitterStreamUrl;
        }

        public String GetCurrentUrl(){
            return this.fullTwitterStreamUrl;
        }

        public int Count()
        {
            return this.count;
        }

        private void DoRead()
        {
            String url = this.BuildTwitterUrl();
            if (this.trackKeywords.Count == 0){
                Console.Out.WriteLine("warning : 0 track keywords listed, gathering all tweets.");
            }
            WebRequest twitterRequest = (HttpWebRequest)WebRequest.Create(url);
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
                if (tweetReader != null)
                {
                    tweetReader.Abort();
                    }
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
