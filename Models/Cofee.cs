using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Cofee : Website
    {
        // 'http://cafef.vn/timeline/31/trang-1551.chn', ck
        // 'http://cafef.vn/timeline/112/trang-1551.chn', ts
        // 'http://cafef.vn/timeline/35/trang-1551.chn', bds
        // 'http://cafef.vn/timeline/36/trang-1.chn', dn
        // 'http://cafef.vn/timeline/34/trang-1.chn', nh
        // 'http://cafef.vn/timeline/32/trang-1.chn', tcqt
        // 'http://cafef.vn/timeline/33/trang-1.chn', vm
        // 'http://cafef.vn/timeline/114/trang-1.chn', s
        // 'http://cafef.vn/timeline/39/trang-1.chn', tt
        // 12
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
             if (type == "ck")
             {
                 SetUrl("http://cafef.vn/timeline/31/trang-1.chn");
             }
             else if (type == "ts")
             {
                 SetUrl("http://cafef.vn/timeline/112/trang-1.chn");
             }
             else if (type == "bds")
             {
                 SetUrl("http://cafef.vn/timeline/35/trang-1.chn");
             }
             else if (type == "dn")
             {
                 SetUrl("http://cafef.vn/timeline/36/trang-1.chn");
             }
             else if (type == "nh")
             {
                 SetUrl("http://cafef.vn/timeline/34/trang-1.chn");
             }
             else if (type == "tcqt")
             {
                 SetUrl("http://cafef.vn/timeline/32/trang-1.chn");
             }
             else if (type == "vm")
             {
                 SetUrl("http://cafef.vn/timeline/33/trang-1.chn");
             }
             else if (type == "s")
             {
                 SetUrl("http://cafef.vn/timeline/114/trang-1.chn");
             }
             else if (type == "tt")
             {
                 SetUrl("http://cafef.vn/timeline/39/trang-1.chn");
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
                var n = await GetContents("http://cafef.vn/" + links[i]);
                n.Url = "http://cafef.vn/" + links[i];
                news.Add(n);
            }
            return news;
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
            title = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"left_cate\")]/*[contains(@class, \"title\")]") != null ? doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"left_cate\")]/*[contains(@class, \"title\")]").InnerText : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sapo')]") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'sapo')]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("//*[@id=\"mainContent\"]/p") != null ? doc.DocumentNode.SelectNodes("//*[@id=\"mainContent\"]/p") : null;
            foreach (var para in paragraphs)
            {
                contents += para != null ? para.InnerText : string.Empty;
            }
            author = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]").InnerText : string.Empty;
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
        private async Task<string> GetTemplate(string url)
        {
            var doc = await GetDocuments(url);
            return doc.DocumentNode.InnerHtml;
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
    }
}