// /html/body/form/div[3]/div/div/div/div[1]/div/div/div/div/div[1]/div/div[2]/h2/a
///html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h1
using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crawler.Models
{
    public class DanTri : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            this.SetUrl("https://dantri.com.vn/");
            if (type == "xh" || type == "")
            {
                this.Type = "Khoa học và công nghệ";
                SetUrl(GetUrl() + "/xa-hoi.htm");
            }
            if (type == "gd")
            {
                this.Type = "Khoa học và công nghệ";
                SetUrl(GetUrl() + "/giao-duc-khuyen-hoc.htm");
            }
           

            return await GetNews(quantity);
        }

        private async Task<List<New>> GetNews(int quantity)
        {
            List<string> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                var n = await GetContents("https://dantri.com.vn" + links[i]);
                n.Url = links[i];
                news.Add(n);
            }
            return news;
        }

        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var childArticles = docs.DocumentNode.SelectNodes("/html/body/div[6]/div[1]/div[1]/div[1]/div[1]/div/div/h2/a");
            // var childArticles = docs.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[1]/section/div/article/figure/a") == null ?
            // docs.DocumentNode.SelectNodes("/html/body/div[1]/div/div[3]/div[1]/div/section/div/article/figure/a") : null;
            List<string> links = new List<string>();
            // links.Add(mainArticle);
            foreach (var a in childArticles)
            {
                links.Add(a.Attributes["href"].Value);
            }

            return links;
        }

        private async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            // client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
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
            string title = "", description = "", contents = "", author = "", source = string.Empty, rendered = "", tag = "", categories = "";
            title = doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h1").InnerText : doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[1]/article/header/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[1]/article/header/h1").InnerText.Trim() : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h2") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h2").InnerText.Trim() : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/div[4]/p") != null ? doc.DocumentNode.SelectNodes("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/div[4]/p") : null;
            for (int i = 0; i < paragraphs.Count; i++)
            {
                contents += paragraphs[i] != null ? paragraphs[i].InnerText.Trim() : string.Empty;
                if (i == paragraphs.Count - 2 || i == paragraphs.Count - 1)
                {
                    tag += paragraphs[i] != null ? paragraphs[i].InnerText.Trim() : string.Empty;
                }
            }
            // var tags = doc.DocumentNode.SelectNodes("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/div[1]/div/div[2]/div/a");
            // for (int i = 0; i < tags.Count - 1; i++)
            // {
            // tag += (tags[i] != null ? tags[i].InnerText.Trim() : string.Empty) + "%";
            // }
            // tag += (tags[tags.Count - 1] != null ? tags[tags.Count - 1].InnerText.Trim() : string.Empty);
            categories = this.Type;
            author = paragraphs[paragraphs.Count - 1].InnerText.Trim();
            source = "https://dantri.com.vn/";
            tag += source;
            // description = source + " - " + description;
            rendered = doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/div[4]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/div[4]").OuterHtml : string.Empty;
            return new New("", "", "", "", "", "");
            
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
            //NOTE remove author
            var author = new Regex("(style=).+\n.+(/p>)", RegexOptions.Multiline);
            var matched = author.Match(n.Rendered);
            if (matched.Success)
            {
                n.Rendered = n.Rendered.Replace(matched.Value, "></p>");
            }
            //NOTE remove strong
            n.Rendered = n.Rendered.Replace("<strong>", "<p>");
            n.Rendered = n.Rendered.Replace("</strong>", "</p>");
            n.Contents = n.Contents.Replace("&nbsp", "");
            n.Contents = n.Contents.Replace("&nbsp;", "");
            return n;
        }
    }
}