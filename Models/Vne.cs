using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace Crawler.Models
{
    public class LinkObj
    {
        public string Link { get; set; }
        public string Img { get; set; }
        public string Title { get; set; }
    }
    public class Vne : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "kd")
            {
                SetUrl(GetUrl() + "/kinh-doanh");
                this.Type = "Kinh doanh";
                this.Categories = "38";
            }
            else if (type == "bds")
            {
                SetUrl(GetUrl() + "/kinh-doanh/bat-dong-san");
                this.Type = "Bất động sản";
                this.Categories = "36";
            }
            else if (type == "ck")
            {
                SetUrl(GetUrl() + "/kinh-doanh/chung-khoan");
                this.Type = "Chứng khoán";
                this.Categories = "34";
            }
            else if (type == "gt")
            {
                SetUrl(GetUrl() + "/giai-tri");
                this.Type = "Giải trí";
                this.Categories = "32";
            }
            else if (type == "tt")
            {
                SetUrl(GetUrl() + "/the-thao");
                this.Type = "Thể thao";
                this.Categories = "31";
            }
            else if (type == "tg")
            {
                SetUrl(GetUrl() + "/the-gioi");
                this.Type = "Thế giới";
                this.Categories = "33-110";

            }
            else if (type == "ts")
            {
                SetUrl(GetUrl() + "/thoi-su");
                this.Type = "Thời sự";
                this.Categories = "139-110";

            }
            else if (type == "kh")
            {
                SetUrl(GetUrl() + "/khoa-hoc");
                this.Type = "Khoa học";
                this.Categories = "152-153";

            }
            else if (type == "sh")
            {
                SetUrl(GetUrl() + "/so-hoa");
                this.Type = "Số hóa";
                this.Categories = "152-113";

            }
            return await GetNews(quantity);
        }

        private async Task<List<New>> GetNews(int quantity)
        {
            // SetUrl(GetUrl() + "/kinh-doanh");
            List<LinkObj> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                try
                {
                    var n = await GetContents(links[i]);
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
            List<LinkObj> links = new List<LinkObj>();
            try
            {
                var mainArticle = docs.DocumentNode.SelectSingleNode("/html/body/section[2]/article/h1/a[1]").Attributes["href"].Value;
                var mainImg = docs.DocumentNode.SelectSingleNode("/html/body/section[2]/article/div/a/img").Attributes["src"].Value;
                links.Add(new LinkObj
                {
                    Link = mainArticle,
                    Img = mainImg
                });
            }
            catch (System.Exception)
            {
            }
            var childArticles = docs.DocumentNode.SelectNodes("/html/body/section[3]/section[1]/article/h4/a[1]");
            var childImgs = docs.DocumentNode.SelectNodes("/html/body/section[3]/section[1]/article/div/a/img");
            for (int i = 0; i < childArticles.Count; i++)
            {
                links.Add(new LinkObj
                {
                    Link = childArticles[i].Attributes["href"].Value,
                    // Img = childImgs[i].Attributes["src"].Value
                });
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

        private async Task<New> GetContents(LinkObj link)
        {
            var doc = await GetDocuments(link.Link);
            string title = "", description = "", author = "", source = string.Empty;
            title = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/h1") != null ? doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/h1").InnerText : string.Empty;
            description = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/p") != null ? doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/p").InnerText : string.Empty;
            author = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/p/strong") != null ? doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/p/strong").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/p/em") != null ? doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/p/em").InnerText : string.Empty;
            var rendered = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]");
            var desc = rendered.ChildNodes[5];
            var realDesc = "";
            if (desc != null)
            {
                foreach (var item in desc.ChildNodes)
                {
                    realDesc += item.InnerText + "-";
                }
                realDesc = realDesc.Remove(realDesc.Length - 1);
            }
            var arti = rendered.ChildNodes[7];
            var contents = "";
            var artChilds = arti.ChildNodes.Where(e => e.Name == "p" || e.Name == "table").ToList();
            for (int i = 0; i < artChilds.Count; i++)
            {
                if (artChilds[i].InnerHtml.Contains("<strong>"))
                {
                    continue;
                }
                contents += artChilds[i].OuterHtml;
            }
            var auth = arti.ChildNodes[arti.ChildNodes.Count - 2];
            var img = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/table/tbody/tr[1]/td/img");
            string url = img == null ? link.Img : img.Attributes["src"].Value;
            url = url.Split("_")[0];
            if (!url.Contains(".jpg")) url += ".jpg";
            arti.RemoveChild(auth);
            var realAuth = "";
            if (auth.Name == "p")
            {
                realAuth = auth.InnerText.Trim();
            }
            else
            {
                realAuth = author.Trim();
            }
            if (realAuth != "")
            {
                realAuth += "-" + source.Trim();
            }
            else
            {
                realAuth += source;
            }
            // var auth = doc.DocumentNode.SelectSingleNode("/html/body/section[2]/section[1]/section[1]/article/p[contains(@style,'text-align:right;')]");
            // rendered.RemoveChild(auth);
            var newRender = realDesc + contents;
            return new New
            (
 title,
                this.RemoveLink(newRender), "",
                "vnexpress.net-" + realAuth,
                url,
                this.Categories
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
                throw new Exception("normalize " + nameof(New));
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
            // n = Refactor(n);
            return n;
        }

        private New Refactor(New n)
        {
            n.Contents = TrimLine(n.Contents);
            n.Author = TrimLine(n.Author);
            n.Source = TrimLine(n.Source);
            n.Description = TrimLine(n.Description);
            n.Title = TrimLine(n.Title);
            n.Rendered = TrimLine(n.Rendered);
            return n;
        }

        private string TrimLine(string str)
        {
            str = str.Replace("\\n", "");
            str = str.Replace("\\t", "");
            return str;
        }
    }
}