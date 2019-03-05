using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace ConsoleAppWebCrawler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                string url = args[0];
                int hops;

                if (int.TryParse(args[1], out hops))
                {
                    List<string> result = GetUrlAndHTML(url, hops);

                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("Result Url: " + result[0]);
                }
                else
                {
                    Console.WriteLine("Hops must be numeric.");
                }

            }
            else
            {
                Console.WriteLine("Make sure to specify 2 arguments.");
            }
        }

        private static List<string> FindHrefs(string input)
        {
            List<string> urls = new List<string>();
            Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match match;
            for (match = regex.Match(input); match.Success; match = match.NextMatch())
            {
                if (match.Groups.Count > 1)
                {
                    urls.Add(match.Groups[1].ToString());
                }
            }
            return urls;
        }

        private static List<string> GetUrlAndHTML(string url, int hops)
        {
            WebClient client = new WebClient();
            string downloadString = client.DownloadString(url);
            List<string> urls = FindHrefs(downloadString);

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Initial Web Request");
            Console.WriteLine("URL: " + url);
            Console.WriteLine("Hops: " + hops);

            foreach (string urlToNavigate in urls)
            {
                if (isValidUrl(urlToNavigate))
                {
                    return GetUrlAndHTML(urlToNavigate, hops, new List<string>());
                }
            }
            return null;
        }

        private static List<string> GetUrlAndHTML(string url, int hops, List<string> urlsVisited)
        {
            WebClient client = new WebClient();
            string downloadString = client.DownloadString(url);
            List<string> urls = FindHrefs(downloadString);
            urlsVisited.Add(url);

            if (hops > 0)
            {
                foreach (string urlToNavigate in urls)
                {
                    if (!urlsVisited.Contains(urlToNavigate) && isValidUrl(urlToNavigate))
                    {
                        Console.WriteLine("----------------------------------------");
                        Console.WriteLine("Next Request");
                        Console.WriteLine("URL: " + urlToNavigate);
                        Console.WriteLine("Hops: " + hops);

                        return GetUrlAndHTML(urlToNavigate, hops - 1, urlsVisited);
                    }
                }

                Console.WriteLine("----------------------------------------");
                Console.WriteLine("No URL found");
                return new List<string>() { url, downloadString };
            }
            else
            {
                // end of hops
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("End of hops");
                return new List<string>() { url, downloadString };
            }
        }

        private static bool isValidUrl(string url)
        {
            if (url.StartsWith("http"))
                return true;
            return false;
        }
    }
}
;