using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Bds : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "chcc")
            {
                SetUrl("https://batdongsan.com.vn/can-ho-chung-cu");
            }
            return await GetNews(quantity);
        }
        private async Task<List<New>> GetNews(int quantity)
        {
            List<string> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                var n = await GetContents("https://batdongsan.com.vn" + links[i]);
                n.Url = "https://batdongsan.com.vn" + links[i];
                news.Add(n);
            }
            return news;
        }

        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("//*[@id=\"form1\"]/div[4]/div[8]/div/div[1]/div/ul/li/div[2]/div[1]/h3/a");
            List<string> links = new List<string>();
            foreach (var a in articles)
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
            string title = "", description = "", contents = "", author = "", source = string.Empty;
            title = doc.DocumentNode.SelectSingleNode("//*[contains(@class, \"prj_noidung\")]/h2/strong") != null ? doc.DocumentNode.SelectSingleNode("///*[@id=\"form1\"]/div[4]/div[8]/div[1]/h2/strong").InnerText : string.Empty;
            // description = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("//*[@id=\"form1\"]/div[4]/div[8]/div[1]/p");
            foreach (var para in paragraphs)
            {
                contents += para != null ? para.InnerText : string.Empty;
            }
            // author = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/strong") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/strong").InnerText : string.Empty;
            // source = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/em") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"left_calculator\"]/article/p/em").InnerText : string.Empty;
            return new New{
                Author = author,
                Title = title,
                Description = description,
                Contents = contents,
                Source = source,
            };
        }

        public override string GetUrl()
        {
            return this.Url;
        }

        public override bool SetUrl(string url)
        {
            try
            {
                this.Url = url;
                return true;
            }
            catch (System.Exception)
            {
                
                throw;
            }
            return false;
        }

        public override Task<string> Template(string url)
        {
            return GetTemplate(url);
        }
    }
}