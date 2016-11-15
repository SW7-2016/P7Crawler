using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI.objects
{
    public class Site
    {
        public string url;
        public Host parent;

        public Site(string Url, List<Host> Hosts)
        {
            MainWindow.allUrls.Add(this);

            url = Url;
            string domain = domainFinder(url);
            foreach (var host in Hosts)
            {
                if (host.domain == domain)
                {
                    parent = host;
                    break;
                }
            }
            if (parent == null)
            {
                MainWindow.hosts.Add(new Host(domainFinder(url)));
                foreach (var host in Hosts)
                {
                    if (host.domain == domain)
                    {
                        parent = host;
                        break;
                    }
                }
            }
        }

        public static void Add(string url)
        {
            Boolean isDoublicate = false;

            if (!url.Contains(".jpg")
                && !url.Contains(".jpeg")
                && !url.Contains(".png")
                && !url.Contains(".mp3")
                && !url.Contains(".gif")
                && !url.Contains(".m4a")
                && !url.Contains(".rss")
                && !url.Contains(".svg")
                && !url.Contains("type=mp3")
                && !url.Contains("type=m4a")
                && !url.Contains("type=flag"))
            {
                foreach (var site in MainWindow.allUrls)
                {
                    if (site.url == url)
                        isDoublicate = true;
                }

                if (!isDoublicate)
                {
                    MainWindow.urls.Enqueue(new Site(url, MainWindow.hosts));
                }
            }
        }

        string domainFinder(string URL)
        {
            string temp = URL;
            int k = 0;

            for (int i = 0; i < URL.Count(); i++)
            {
                if (temp[i] == '/')
                {
                    k++;

                    if (k > 2)
                    {
                        temp = temp.Remove(i, temp.Count() - i);
                        break;
                    }
                }
            }
            return temp;
        }

    }
}
