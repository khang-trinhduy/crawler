using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;
using Newtonsoft.Json;

namespace Crawler.Models
{
    public class Vnnet : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "tc" || type == "")
            {
                SetUrl("https://vietnamnet.vn/jsx/loadmore/?domain=desktop&c=kinh-doanh-tai-chinh&p=1&s=30");
                this.Categories = "53-2421";
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
                    var n = await GetContents(links[i].Link, links[i].Img, links[i].Title);
                    news.Add(n);
                    
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            return news;
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
        private async Task<New> GetContents(string url, string img, string title)
        {
            var doc = await GetDocuments(url);
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"ArticleContent\")]");
            var related = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"article-relate\")]");
            contentpage.RemoveChild(related);
            var author = contentpage.ChildNodes.LastOrDefault(e => e.Name == "p" && e.InnerText.Length <= 40);
            if (author != null)
            {
                contentpage.RemoveChild(author);
                
            }
            var auth = "vietnamnet.vn-" + author.InnerText;
            return new New
            (
                title,
                contentpage.InnerHtml,"", 
                auth, img,this.Categories
            );
        }
        private async Task<List<LinkObj>> GetLinks()
        {
            var client = new HttpClient();
            var res = await client.GetAsync(this.Url);
            res.EnsureSuccessStatusCode();
            List<VnnetJson> articles = new List<VnnetJson>();
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                var index = content.IndexOf('=');
                content = content.Substring(index + 1);
                articles = JsonConvert.DeserializeObject<List<VnnetJson>>(content);
            }
            List<LinkObj> links = new List<LinkObj>();
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(new LinkObj{
                    Link = articles[i].Link,
                    Img = articles[i].Image,
                    Title = articles[i].Title
                });
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

        public override New Normalize(New n)
        {
            throw new System.NotImplementedException();
        }
    }

    public class VnnetJson
    {
        public string Image { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
    }
}