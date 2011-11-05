# ESP
esp is a project designed to detect natural disasters from social media
sources (specifically twitter), make predictions of affected areas
and provide alerts to people living in those areas automatically.

# Setup
To run you must create a `Options.cs` file of the following format

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	namespace Tweet
	{
		class Options
		{
			public static String twitterStreamUrl = your_stream_url_here;
			public static String twitterUsername = your_twitter_username;
			public static String twitterPassword = your_twitter_password;
			public static int numberThreads = 4;
		}
	}

# TODO
* Refactor the consumer and producer (relabel and split up, add producer interface)
