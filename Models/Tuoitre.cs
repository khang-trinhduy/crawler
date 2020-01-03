// /html/body/div[1]/div/section/div/div[3]/div[1]/div[2]/ul/li/h2/a
using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace Crawler.Models
{
    public class Tuoitre : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            this.SetUrl("https://tuoitre.vn/");
            if (type == "kd" || type == "")
            {
                this.Type = "Kinh doanh";
                SetUrl(GetUrl() + "/kinh-doanh.htm");
                this.Categories = "38";
            }


            return await GetNews(quantity);
        }

        private async Task<List<New>> GetNews(int quantity)
        {
            List<string> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                try
                {
                var n = await GetContents("https://tuoitre.vn" + links[i]);
                n.Url = links[i];
                news.Add(n);
                    
                }
                catch (System.Exception)
                {
                   continue; 
                }
            }
            return news;
        }

        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            List<string> links = new List<string>();
            var ul = docs.DocumentNode.SelectSingleNode("//ul[contains(@class, \"list-news-content\")]");
            var li = ul.ChildNodes.Where(e => e.Name == "li").ToList();
            for (int i = 0; i < li.Count; i++)
            {
                var a = li[i].ChildNodes.FirstOrDefault(e => e.Name == "a");
                links.Add(a.Attributes["href"].Value);
            }
            // links.Add(mainArticle);
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
            var title = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, \"article-title\")]");
            var contentfck = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"fck\")]");
            var img = contentfck.ChildNodes.FirstOrDefault(e => e.Name == "div").ChildNodes.FirstOrDefault(e => e.Name == "div").ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value;
            var sapo = doc.DocumentNode.SelectSingleNode("//h2[contains(@class, \"sapo\")]");
            var author = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"author\")]");
            var relateContent = contentfck.ChildNodes.LastOrDefault(e => e.Name == "div");
            contentfck.RemoveChild(relateContent);
            var relative = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"relate-container\")]");

            return new New
            (
                title.InnerText,
                "<i>" + sapo.InnerText + "</i>" + contentfck.InnerHtml,
                "",
                "tuoitre.vn-" + author.InnerText,
                img,this.Categories
            );
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