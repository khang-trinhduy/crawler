using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace Crawler.Models
{
    public class Cungcau : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ls" || type == null)
            {
                SetUrl("https://cungcau.vn/lai-suat-ngan-hang-moi-nhat-channel71/");
                this.Categories = "3427";
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
            var contentpage = doc.DocumentNode.QuerySelector("#content");
            title = doc.QuerySelector("#wrapper_all_content > div.content-left > div.single > h1").InnerText;
            var trashy = doc.DocumentNode.QuerySelector("#content > div.explus_related_1404022217.explus_related_1404022217_bottom._tlq");
            
            if (trashy != null)
            {
                contentpage.RemoveChild(trashy);
                
            }
            var des = "<i>" +  doc.DocumentNode.QuerySelector("#wrapper_all_content > div.content-left > div.single > div.sapo").InnerText + "</i>";
            var auth = "cungcau.vn";
            var image = Bank.GetLink(title);
            return new New
            (
                title,
                contentpage.InnerHtml,des, 
                auth, image,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[1]/div[6]/div[4]/div/div[2]/div/div/h2/a");
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