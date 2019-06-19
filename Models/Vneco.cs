using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Vneco : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ts")
            {
                SetUrl("http://vneconomy.vn/timeline/9920/trang-1.htm");
            }
            else if (type == "tc")
            {
                SetUrl("http://vneconomy.vn/timeline/6/trang-1.htm");
            }
            else if (type == "ck")
            {
                SetUrl("http://vneconomy.vn/timeline/7/trang-1.htm");
            }
            else if (type == "dn")
            {
                SetUrl("http://vneconomy.vn/timeline/5/trang-1.htm");
            }
            else if (type == "do")
            {
                SetUrl("http://vneconomy.vn/timeline/17/trang-1.htm");
            }
            else if (type == "tt")
            {
                SetUrl("http://vneconomy.vn/timeline/19/trang-1.htm");
            }
            else if (type == "tg")
            {
                SetUrl("http://vneconomy.vn/timeline/99/trang-1.htm");
            }
            else if (type == "css")
            {
                SetUrl("http://vneconomy.vn/timeline/16/trang-1.htm");
            }
            else if (type == "x3")
            {
                SetUrl("http://vneconomy.vn/timeline/23/trang-1.htm");
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
                var n = await GetContents("http://vneconomy.vn/" + links[i]);
                n.Url = "http://vneconomy.vn/" + links[i];
                news.Add(n);
            }
            return news;
        }
        private async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            var mainPage = await client.GetStringAsync(url);
            HtmlDocument docs = new HtmlDocument();
            docs.LoadHtml(mainPage);
            return docs;
        }
        private async Task<New> GetContents(string url)
        {
            var doc = await GetDocuments(url);
            string title = "", description = "", contents = "", author = "", source = string.Empty;
            title = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"contentleft\")]/*[contains(@class, \"title\")]") != null ? doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"left_cate\")]/*[contains(@class, \"title\")]").InnerText : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sapo')]") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sapo')]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("//*[@id=\"mainContent\"]/p") != null ? doc.DocumentNode.SelectNodes("//*[@id=\"mainContent\"]/p") : null;
            foreach (var para in paragraphs)
            {
                contents += para != null ? para.InnerText : string.Empty;
            }
            author = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]").InnerText : string.Empty;
            return new New
            {
                Author = author,
                Title = title,
                Description = description,
                Contents = contents,
                Source = source,
            };
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/li/h3/a");
            List<string> links = new List<string>();
            foreach (var a in articles)
            {
                links.Add(a.Attributes["href"].Value);
            }

            return links;
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
                return false;
            }
        }
        public override Task<string> Template(string url)
        {
            return GetTemplate(url);
        }
        private async Task<string> GetTemplate(string url)
        {
            var doc = await GetDocuments(url);
            return doc.DocumentNode.InnerHtml;
        }
    }
}