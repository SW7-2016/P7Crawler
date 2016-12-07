using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReviewCrawler.Sites
{
    abstract class Site : Host
    {
        public override abstract bool Parse(string siteData, string sQueueData);
        public override abstract void Crawl(string siteData, string sQueueData);

        //Looks in a string for a regex match, and returns the match, witout the string identifiers
        public string regexMatch(string data, string start, string end)
        {
            Match match = Regex.Match(data, start + ".*?" + end, RegexOptions.Singleline);

            if (match.Success)
            {
                return match.Value.Replace(start, "").Replace(end, "").Trim();
            }
            else
            {
                return "";
            }
        }

        //Looks in a string for regex matches, and returns the matches, witout the string identifiers
        public string[] regexMatches(string data, string start, string end)
        {
            MatchCollection matches = Regex.Matches(data, start + ".*?" + end);

            string[] result = new string[matches.Count];
            int i = 0;

            foreach (Match match in matches)
            {
                result[i++] = match.Value.Replace(start, "").Replace(end, "").Trim();
            }

            return result;
        }
    }
}
