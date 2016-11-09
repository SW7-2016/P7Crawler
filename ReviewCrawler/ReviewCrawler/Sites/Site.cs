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
        public override abstract bool Parse(string siteData);
        public override abstract void CrawlPage(string siteData);
        public override abstract string GetSiteKey(string url);


        public string regexMatch(string data, string start, string end)
        {
            Match match = Regex.Match(data, start + ".*?" + end);
            return match.Value.Replace(start, "").Replace(end, "").Trim();
        }

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

        //Item refers to product/review links
        public List<string> GetItemLinks(string siteData, string firstTagForItemLink, string secondTagForItemLink, string splitTag, bool reverse)
        {
            List<string> itemLinks = new List<string>();

            string[] splitOnItems = siteData.Split(new string[] { splitTag }, StringSplitOptions.None);
            string tempItemLink;

            foreach (var item in splitOnItems)
            {
                tempItemLink = GetSearchLinks(item, firstTagForItemLink, secondTagForItemLink, reverse);

                if (tempItemLink != domainUrl)
                {
                    itemLinks.Add(tempItemLink);
                }

            }
            return itemLinks;
        }

        public string GetSearchLinks(string siteData, string firstIdentifier, string secondIdentifier, bool reverse)
        {
            string newSearchLink = "";
            string tempString = "";
            bool linkFound = false;
            bool copyLink = false;
            siteData = siteData.ToLower();

            string[] lines = siteData.Split('\n');

            int firstLineModifier = 0;
            int secondLineModifier = 1;

            if (reverse)
            {
                firstLineModifier = 1;
                secondLineModifier = 0;
            }

            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i + firstLineModifier].Contains(firstIdentifier) && lines[i + secondLineModifier].Contains(secondIdentifier))
                {
                    for (int j = 0; j < lines[i + secondLineModifier].Length; j++)
                    {
                        if (copyLink == true)
                        {
                            if ((lines[i + secondLineModifier])[j] == '"')
                            {
                                break;
                            }
                            tempString += (lines[i + secondLineModifier])[j];
                        }

                        if ((lines[i + secondLineModifier])[j] == 'h'
                            && (lines[i + secondLineModifier])[j + 1] == 'r'
                            && (lines[i + secondLineModifier])[j + 2] == 'e'
                            && (lines[i + secondLineModifier])[j + 3] == 'f')
                        {
                            linkFound = true;
                        }
                        if (linkFound == true && (lines[i + secondLineModifier])[j] == '"')
                        {
                            copyLink = true;
                        }

                    }
                }
                if (copyLink == true)
                {
                    break;
                }
            }

            newSearchLink = (domainUrl + tempString);

            return newSearchLink;
        }

    }
}
