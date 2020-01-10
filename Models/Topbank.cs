using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Topbank : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "nh" || type == "")
            {
                SetUrl("https://topbank.vn/tin-tuc/ngan-hang-bao-hiem");
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
                    var n = await GetContents("https://topbank.vn/" + links[i].Link, links[i].Img);
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
        private async Task<New> GetContents(string url, string img)
        {
            var doc = await GetDocuments(url);
            string title = "", source = string.Empty;
            var contentpage = doc.DocumentNode.SelectSingleNode("/html/body/div[5]/div[4]/div/div[1]/div/div[2]");
            title = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, \"heading-detail\")]").InnerText;
            var auth = "topbank.vn-";
            var author = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"author\")]");
            if (author != null)
            {
                contentpage.RemoveChild(author);
                auth += author.InnerText;
            }
            var index = img.IndexOf("/crop");
            var image = img.Substring(0, index) + img.Substring(index + 13);
            return new New
            (
                title,
                contentpage.InnerHtml,"", 
                auth, image,this.Categories
            );
        }
        private async Task<List<LinkObj>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[5]/div[4]/div/div[1]/div/ul/li/div[1]/a");
            List<LinkObj> links = new List<LinkObj>();
            for (int i = 0; i < articles.Count; i++)
            {
                var image = articles[i].ChildNodes.FirstOrDefault(e => e.Name == "img");
                links.Add(new LinkObj{Link = articles[i].Attributes["href"].Value, Img = image.Attributes["src"].Value});
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