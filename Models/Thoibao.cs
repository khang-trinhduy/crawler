using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Thoibao : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "tb" || type == "")
            {
                SetUrl("http://thoibaotaichinhvietnam.vn/pages/nhip-song-tai-chinh-3.aspx");
                this.Categories = "53-2421";
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
                    var n = await GetContents("http://thoibaotaichinhvietnam.vn" + links[i]);
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
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"article-ds\")]");
            title = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"article-header\")]").ChildNodes.FirstOrDefault(e => e.Name == "h1").InnerText;
            var author = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"tacgia\")]");
            if (author != null)
            {
                contentpage.RemoveChild(author);
            }
            var auth = "thoibaotaichinhvietnam.vn-" + author.InnerText.Trim();
            var image = "http://thoibaotaichinhvietnam.vn" + doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"img-page\")]").ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value;
            return new New
            (
                title,
                contentpage.InnerHtml,"", 
                auth, image,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/form/div[9]/div[3]/div/div/div[1]/div[3]/div[1]/div[1]/div[2]/div[1]/div[1]/div/div[1]/div/div[1]/div/div/div/div/a");
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