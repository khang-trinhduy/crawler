using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Worldbank : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "nh" || type == "")
            {
                SetUrl("http://www.thegioinganhang.vn/");
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
                    var n = await GetContents(links[i].Link, links[i].Img);
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
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"single\")]");
            var titleElem = contentpage.ChildNodes.FirstOrDefault(e => e.Name == "h2");
            if (titleElem != null)
            {
                contentpage.RemoveChild(titleElem);
            }
            title = titleElem.InnerText;
            var trashy = contentpage.ChildNodes.Where(e => e.Name == "div").ToList();
            foreach (var trash in trashy)
            {
                contentpage.RemoveChild(trash);
            }
            var detail = contentpage.ChildNodes.FirstOrDefault(e => e.Name == "p" && e.HasClass("details"));
            if (detail != null)
            {
                contentpage.RemoveChild(detail);
            }
            var auth = "thegioinganhang.vn";
            var index = img.LastIndexOf('-');
            var image = img.Substring(0, index) + ".jpg";
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
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[1]/div[3]/div[2]/div/a");
            List<LinkObj> links = new List<LinkObj>();
            for (int i = 0; i < articles.Count; i++)
            {
                try
                {
                    var img = articles[i].ChildNodes.FirstOrDefault(e => e.Name == "img");
                    links.Add(new LinkObj{Link = articles[i].Attributes["href"].Value, Img = img.Attributes["src"].Value});
                }
                catch (System.Exception)
                {
                    continue;
                }
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