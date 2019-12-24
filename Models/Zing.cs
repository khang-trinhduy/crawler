// /html/body/div[6]/section[2]/div/div/article/header/p[1]/a links
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Zing : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "cn")
            {
                SetUrl("https://news.zing.vn/cong-nghe.html");
                Type = "Công nghệ";
            }
            else if (type == "tc")
            {
                SetUrl("https://news.zing.vn/cong-nghe.html");
                Type = "Tài chính";
            }
            else if (type == "sea")
            {
                SetUrl("https://news.zing.vn/SEA-Games-30.html");
                Type = "SEA Game";
            }
            else if (type == "ts")
            {
                SetUrl("https://news.zing.vn/thoi-su.html");
                Type = "Thời sự";
            }
            else if (type == "pl")
            {
                SetUrl("https://news.zing.vn/phap-luat.html");
                Type = "Pháp luật";
            }
            else if (type == "tg")
            {
                SetUrl("https://news.zing.vn/the-gioi.html");
                Type = "Thế giới";
            }
            else if (type == "xb")
            {
                SetUrl("https://news.zing.vn/xuat-ban.html");
                Type = "Xuất bản";
            }
            else if (type == "kd")
            {
                SetUrl("https://news.zing.vn/kinh-doanh-tai-chinh.html");
                Type = "Kinh doanh";
            }
            else if (type == "gt")
            {
                SetUrl("https://news.zing.vn/giai-tri.html");
                Type = "Giải trí";
            }
            else if (type == "ds")
            {
                SetUrl("https://news.zing.vn/doi-song.html");
                Type = "Đời sống";
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
                var n = await GetContents("https://news.zing.vn" + links[i]);
                n.Url = "https://news.zing.vn" + links[i];
                news.Add(n);
            }
            return news;
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var uppers = docs.DocumentNode.SelectNodes("//*[@id=\"topnews\"]/article/header/p[1]/a");
            var lowers = docs.DocumentNode.SelectNodes("//*[@id=\"mostview\"]/article/header/p[1]/a");
            // var articles = docs.DocumentNode.SelectNodes("/html/body/div[6]/section[2]/div/div/article/header/p[1]/a");
            List<string> links = new List<string>();
            foreach (var a in uppers)
            {
                links.Add(a.Attributes["href"].Value);
            }
            foreach (var a in lowers)
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
        private async Task<New> GetContents(string url)
        {
            var doc = await GetDocuments(url);
            string title = "", description = "", contents = "", author = "", source = string.Empty, rendered = string.Empty;
            title = doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/header/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/header/h1").InnerText : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/p[1]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/p[1]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("/html/body/div[6]/article/section[1]/div[1]/p") != null ? doc.DocumentNode.SelectNodes("/html/body/div[6]/article/section[1]/div[1]/p") : null;
            foreach (var para in paragraphs)
            {
                contents += para != null ? para.InnerText : string.Empty;
            }
            author = doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/header/ul/li[1]/a") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/header/ul/li[1]/a").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/div[2]/p[2]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/div[2]/p[2]").InnerText : string.Empty;
            rendered = doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/div[1]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[6]/article/section[1]/div[1]").OuterHtml : string.Empty;
            return new New
           (
                "","","","","",""
            );
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

        public override New Normalize(New n)
        {
            if (n == null)
            {
                throw new Exception(nameof(n));
            }
            //NOTE remove tag
            var links = new Regex("(<a).+\n*\t*.+(</a>)");
            var matched = links.Matches(n.Rendered);
            if (matched != null && matched.Count > 0)
            {
                for (int i = 0; i < matched.Count; i++)
                {
                    n.Rendered = n.Rendered.Replace(matched[i].Value, "");
                }
            }
            var source = new Regex("(<p class=\"source).+\n*\t*.+(</p>)");
            var author = new Regex("(<p class=\"author).+\n*\t*.+(</p>)");
            var tags = new Regex("(<p class=\"author).+\n*\t*.+(</p>)");
            n.Rendered = Remove(source, n.Rendered);
            n.Rendered = Remove(author, n.Rendered);
            n.Rendered = n.Rendered.Replace("<b>", "<p>");
            n.Rendered = n.Rendered.Replace("<strong>", "<p>");
            n.Rendered = n.Rendered.Replace("</b>", "</p>");
            n.Rendered = n.Rendered.Replace("</strong>", "</p>");
            n.Rendered = n.Rendered.Replace("<p>Từ Khóa:</p>", "");
            return n;
        }
        private string Remove(Regex re, string str)
        {
            var matched = re.Match(str);
            if (matched.Success)
            {
                str = str.Replace(matched.Value, "");
            }
            return str;
        }
    }
}