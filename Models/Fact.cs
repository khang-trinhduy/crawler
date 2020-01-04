using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Fact : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "tt" || type == "")
            {
                SetUrl("https://factcheck.vn/the-thao");
                this.Categories = "2135-31";
            }
            else if (type == "kt")
            {
                SetUrl("https://factcheck.vn/kinh-te");
                this.Categories = "2135-35";
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
                    var n = await GetContents(links[i]);
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
            var shits = doc.DocumentNode.SelectNodes("//div[contains(@class, \"cs-custom\")]");
            var realShit = shits.LastOrDefault();
            title = doc.DocumentNode.SelectSingleNode("//p[contains(@class, \"block-heading\")]").InnerText;
            var blockImg = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"block-img\")]");
            var image = blockImg.ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value;
            if (!image.Contains("http"))
            {
                image = "https://factcheck.vn" + image;
            }
            return new New
            (
                title,
                realShit.InnerHtml,"", 
                "fackcheck.vn", image,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/main/div/div/div[1]/section[2]/div/div[2]/ul/li/div[1]/a");
            List<string> links = new List<string>();
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(articles[i].Attributes["href"].Value);
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