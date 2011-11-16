using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TweetClassifier
{
    public class TweetClassifier
    {
        public Dictionary<String, List<String>> whitelists = new Dictionary<String, List<String>>();
        public Dictionary<String, List<String>> blacklists = new Dictionary<String, List<String>>();
        public Dictionary<String, Dictionary<String, int>> keywordRatings = new Dictionary<String, Dictionary<String, int>>();
        public Dictionary<String, int> tweetThreshold = new Dictionary<String, int>();
        private String basedir = "./ClassifierData";

        public void loadKeywordProfile(String keyword){
            String fullpath = basedir + "/" + keyword + "/";
            StreamReader sr;
            // load whitelist
            if (File.Exists(fullpath + "whitelist"))
            {
                sr = new StreamReader(fullpath + "whitelist");
                String text = sr.ReadToEnd();
                whitelists[keyword] = text.Split('\n').ToList();
            }
            // load blacklist
            if (File.Exists(fullpath + "blacklist"))
            {
                sr = new StreamReader(fullpath + "blacklist");
                String text = sr.ReadToEnd();
                blacklists[keyword] = text.Split('\n').ToList();
            }
            // load up keyword ratings
            if (File.Exists(fullpath + "ratings"))
            {
                keywordRatings[keyword] = new Dictionary<String, int>();
                sr = new StreamReader(fullpath + "ratings");
                String text = sr.ReadToEnd();
                foreach (String kwpair in text.Split('\n'))
                {
                    String[] p = kwpair.Split(' ');
                    if (p.Count() == 2) {
                        keywordRatings[keyword][p[0]] = int.Parse(p[1]);
                    }
                }
            }
            // load up tweet threshold
            if (File.Exists(fullpath + "threshold"))
            {
                sr = new StreamReader(fullpath + "threshold");
                String text = sr.ReadToEnd().Trim();
                tweetThreshold[keyword] = int.Parse(text);
            }
        }

        public void loadKeywordProfile()
        {
            // attempts to load all keyword profiles
            String[] keywords = Directory.GetDirectories(basedir);
            foreach (String keyword in keywords)
            {
                loadKeywordProfile(keyword);
            }
        }

        public String getBasedir()
        {
            return basedir;
        }
        public void setBasedir(String basedir)
        {
            this.basedir = basedir;
        }

        public void addWhitelist(String keyword, List<String> whitelist)
        {
            whitelists[keyword] = whitelist;
        }

        public void addBlacklist(String keyword, List<String> whitelist)
        {
            whitelists[keyword] = whitelist;
        }

        public static bool filterUser(String tweet, String keyword)
        {
            // filters out non username keywords
            Regex r = new Regex("(.*)(@[a-zA-Z0-9]+)(.*)");
            Match m = r.Match(tweet);
            if (m.Success)
            {
                Group g1 = m.Groups[1];
                Group g2 = m.Groups[1];
                if (g1.Value.Contains(keyword) || g2.Value.Contains(keyword))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool filterFormWhitelist(String tweet, String keyword, List<String> validForms)
        {
            String[] words = tweet.Split(' ');
            foreach (String word in words)
            {
                if (word.Contains(keyword))
                {
                    if (validForms.Contains(word))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool filterFormWhitelist(String tweet, String keyword)
        {
            if (whitelists.ContainsKey(keyword))
            {
                return filterFormWhitelist(tweet, keyword, whitelists[keyword]);
            }
            else
            {
                // default to false if no white list
                return false;
            }
        }

        public bool filterFormBlacklist(String tweet, String keyword, List<String> invalidForms)
        {
            // returns false if word IS on blacklist
            String[] words = tweet.Split(' ');
            foreach (String word in words)
            {
                if (word.Contains(keyword))
                {
                    if (invalidForms.Contains(word))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool filterFormBlacklist(String tweet, String keyword)
        {
            if (blacklists.ContainsKey(keyword))
            {
                return filterFormBlacklist(tweet, keyword, blacklists[keyword]);
            }
            else
            {
                // default to true if no white list
                return true;
            }
        }

        public bool filter(String tweet, String keyword)
        {
            // first check if non-username keyword
            if (filterUser(tweet, keyword))
            {
                // check if on whitelist, if not then check blacklist
                if (filterFormWhitelist(tweet, keyword))
                {
                    return true;
                }
                else
                {
                    // check if on blacklist, if it is then returns false
                    if (filterFormBlacklist(tweet, keyword))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public int score(String tweet, String keyword)
        {
            int score = 0;
            if(keywordRatings.ContainsKey(keyword))
            {
                String[] words = tweet.Split(' ');
                foreach (String word in words)
                {
                    if (keywordRatings[keyword].ContainsKey(word))
                    {
                        score += keywordRatings[keyword][word];
                    }
                }
            }
            return score;
        }

        public bool classify(String tweet, String keyword)
        {
            int threshold = 0;
            if (tweetThreshold.ContainsKey(keyword))
            {
                threshold = tweetThreshold[keyword];
            }
            if (filter(tweet, keyword) && score(tweet, keyword) >= threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
