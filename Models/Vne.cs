using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crawler.Models
{
    public class Vne : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "kd")
            {
                SetUrl(GetUrl() + "/kinh-doanh");
            }
            else if (type == "bds")
            {
                SetUrl(GetUrl() + "/kinh-doanh/bat-dong-san");
            }
            else if (type == "ck")
            {
                SetUrl(GetUrl() + "/kinh-doanh/chung-khoan");
            }
            return await GetNews(quantity);
        }

        private async Task<List<New>> GetNews(int quantity)
        {
            // SetUrl(GetUrl() + "/kinh-doanh");
            List<string> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                var n = await GetContents(links[i]);
                n.Url = links[i];
                news.Add(n);
            }
            return news;
        }

        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var mainArticle = docs.DocumentNode.SelectSingleNode("/html/body/section[2]/article/h1/a[1]").Attributes["href"].Value;
            var childArticles = docs.DocumentNode.SelectNodes("/html/body/section[3]/section[1]/article/h4/a[1]");
            List<string> links = new List<string>();
            links.Add(mainArticle);
            foreach (var a in childArticles)
            {
                links.Add(a.Attributes["href"].Value);
            }

            return links;
        }

        private async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
            var mainPage = await client.GetStringAsync(url);
            HtmlDocument docs = new HtmlDocument();
            docs.LoadHtml(mainPage);
            return docs;
        }

        private async Task<string> GetTemplate(string url)
        {
            var doc = await GetDocuments(url);
            return doc.DocumentNode.InnerHtml;
        }

        private async Task<New> GetContents(string url)
        {
            var doc = await GetDocuments(url);
            string title = "", description = "", contents = "", author = "", source = string.Empty, rendered = "";
            title = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/h1") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/h1").OuterHtml : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]").OuterHtml : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("//*[@id=\"left_calculator\"]/article/p") != null ? doc.DocumentNode.SelectNodes("//*[@id=\"left_calculator\"]/article/p") : null;
            foreach (var para in paragraphs)
            {
                contents += para != null ? para.OuterHtml : string.Empty;
            }
            author = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/strong") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/strong").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/em") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/em").InnerText : string.Empty;
            rendered = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sidebar_1')]/article") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sidebar_1')]/article").OuterHtml : string.Empty;
            return Normalize( new New
            {
                Author = author,
                Title = title,
                Description = description,
                Contents = contents,
                Source = source,
                Rendered = rendered
            });
        }

        public override string GetUrl()
        {
            return this.Url;
        }

        public override bool SetUrl(string url = null)
        {
            try
            {
                this.Url = url;
                return true;
            }
            catch (System.Exception)
            {

                throw new Exception("can't get url!");
            }
        }

        public override Task<string> Template(string url)
        {
            return GetTemplate(url);
        }
        /// <summary>
        /// change i,b, strong, ect... to p tag
        /// </summary>
        public override New Normalize(New n)
        {
            if (n == null)
            {
                throw new Exception(nameof(New));
            }
            var author = new Regex("(style=).+\n.+(/p>)", RegexOptions.Multiline);
            var matched = author.Match(n.Rendered);
            if (matched.Success)
            {
                n.Rendered = n.Rendered.Replace(matched.Value, "></p>");
            }
            n.Rendered = n.Rendered.Replace("<strong>", "<p>");
            n.Rendered = n.Rendered.Replace("</strong>", "</p>");
            return n;
        }
    }
}