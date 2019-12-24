// /html/body/form/div[3]/div/div/div/div[1]/div/div/div/div/div[1]/div/div[2]/h2/a
///html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/h1
using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace Crawler.Models
{
    public class TiaSang : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            this.SetUrl("http://tiasang.com.vn/");
            if (type == "cn" || type == "")
            {
                this.Type = "Khoa học và công nghệ";
                SetUrl(GetUrl() + "/khoa-hoc-cong-nghe");
                this.Categories = "152-63";
            }


            return await GetNews(quantity);
        }

        private async Task<List<New>> GetNews(int quantity)
        {
            List<LinkObj> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                try
                {
                    var n = await GetContents(links[i].Link, links[i].Img);
                    n.Url = links[i].Link;
                    news.Add(n);
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            return news;
        }

        private async Task<List<LinkObj>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var childArticles = docs.DocumentNode.SelectNodes("//*[@id=\"dnn_ctr571_ModuleContent\"]/div/div[1]/div/div[1]/a");
            // var childArticles = docs.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[1]/section/div/article/figure/a") == null ?
            // docs.DocumentNode.SelectNodes("/html/body/div[1]/div/div[3]/div[1]/div/section/div/article/figure/a") : null;
            List<LinkObj> links = new List<LinkObj>();
            // links.Add(mainArticle);
            foreach (var a in childArticles)
            {
                var link = new LinkObj();
                link.Link = a.Attributes["href"].Value;
                var img = a.ChildNodes.FirstOrDefault(e => e.Name == "img");
                if (img != null)
                {
                    link.Img = "http://tiasang.com.vn" + img.Attributes["src"].Value;
                }
                links.Add(link);
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

        private async Task<New> GetContents(string url, string img)
        {
            var doc = await GetDocuments(url);
            string title = "", description = "", contents = "", author = "", source = string.Empty, categories = "";
            title = doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/h1").InnerText : doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[1]/article/header/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div[3]/div[1]/article/header/h1").InnerText.Trim() : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/p[2]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/p[2]").InnerText.Trim() : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/div[1]/p") != null ? doc.DocumentNode.SelectNodes("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/div[1]/p") : null;
            for (int i = 0; i < paragraphs.Count; i++)
            {
                contents += paragraphs[i] != null ? paragraphs[i].InnerText.Trim() : string.Empty;
            }

            categories = this.Type;
            author = paragraphs[paragraphs.Count - 1].InnerText.Trim();
            source = "http://tiasang.com.vn/";
            var rendered = doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/div/div/div/div[1]/div/div/div/div[2]/div[1]");
            var trash = rendered.ChildNodes.Where(e => e.Name == "#text").ToList();
            foreach (var item in trash)
            {
                rendered.RemoveChild(item);
            }
            var src = rendered.ChildNodes.LastOrDefault(e => e.ChildNodes != null && e.Name == "p" && e.ChildNodes.FirstOrDefault().Name == "strong");
            var tag = "";
            if (src != null)
            {

                rendered.RemoveChild(src);
                if (!src.FirstChild.InnerText.Contains("-"))
                {
                    tag += src.FirstChild.InnerText;

                }
            }
            var auth = rendered.ChildNodes.LastOrDefault(e => e.ChildNodes != null && e.Name == "p" && e.ChildNodes.FirstOrDefault().Name == "strong");
            if (auth != null)
            {
                rendered.RemoveChild(auth);
                tag += "-" + auth.FirstChild.InnerText;
            }
            return new New
            (
                // Author = author,
                title,
                rendered.InnerHtml,"", 
                "tiasang.com.vn" + "-" + tag,
                img,
                Categories = this.Categories
            );
        }

        private string UpdateSrc(string rendered)
        {
            return rendered.Replace("src=\"", "src=\"http://tiasang.com.vn");
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