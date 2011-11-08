# ESP
esp is a project designed to detect natural disasters from social media
sources (specifically twitter), make predictions of affected areas
and provide alerts to people living in those areas automatically.

# Setup
To run you must create a `Options.cs` file of the following format, the stream
url is just the base url to the stream you are using, the methods used for searching
tend to rely on the filter.json stream method to provide keyword and geo based searching.

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	namespace Tweet
	{
		class Options
		{
			public static String twitterStreamUrl = "https://stream.twitter.com/1/statuses/filter.json";
			public static String twitterUsername = your_twitter_username;
			public static String twitterPassword = your_twitter_password;
			public static int numberThreads = 4;
		}
	}

# TODO
* Consolidate TwitterProducer and TwitterConsumer into a class inherited from "StatusProducer" which will allow for multiple services (Facebook, Twitter, Google+) to be used and produce a uniform "Status" object.
* Use Silverlight/Bing Maps API To plot tweets: http://stackoverflow.com/questions/2162626/silverlight-add-pushpin-to-bing-maps-via-c-sharp
* Come up with basic skeleton for event detection: take tweets by keyword, look for high density clusters with dbscan, consider it an event if more than X tweets supporting the cluster.