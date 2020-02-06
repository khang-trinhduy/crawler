using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace Crawler.Models
{
    public class Viettime : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "tt" || type == null)
            {
                SetUrl("https://viettimes.vn/thi-truong/");
                this.Categories = "134";
            }
            else if (type == "bds")
            {
                SetUrl("https://viettimes.vn/dia-oc-so/");
                this.Categories = "134";
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
                try
                {
                    var n = await GetContents("https://viettimes.vn" + links[i]);
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
            string title = "", source = string.Empty;
            var summary = doc.QuerySelector("div.article__summary").OuterHtml;
            var article = doc.QuerySelector("article").InnerHtml;
            title = doc.QuerySelector("h1.cms-title").InnerText;
            var author = "viettimes";
            var image = doc.DocumentNode.QuerySelector("img.cms-photo").Attributes["src"].Value;
            return new New
            (
                title,
                summary + article,summary, 
                author, image,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/main/div/div[1]/section/div/article/div[1]/h3/a");
            var childs = docs.DocumentNode.SelectNodes("/html/body/main/div/div[1]/section/div/article/div[2]/h3/a");
            List<string> links = new List<string>();
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(articles[i].Attributes["href"].Value);
            }
            for (int i = 0; i < childs.Count; i++)
            {
                links.Add(childs[i].Attributes["href"].Value);
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
}