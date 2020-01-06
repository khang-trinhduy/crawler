using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Tuoitrecuoi : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "gt")
            {
                SetUrl("https://cuoi.tuoitre.vn/giai-tri");
                this.Categories = "32";
            }
            else if (type == "cuoi")
            {
                SetUrl("https://cuoi.tuoitre.vn/doi-cuoi");
                this.Categories = "2298-32";
            }
            else if (type == "tt")
            {
                SetUrl("https://cuoi.tuoitre.vn/tin-tuc");
                this.Categories = "32";
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
            string title = doc.DocumentNode.SelectSingleNode("//article[contains(@class, \"art-header\")]").ChildNodes.FirstOrDefault(e => e.Name == "h1").InnerText;
            var contentpage = doc.DocumentNode.SelectSingleNode("//article[contains(@class, \"art-body\")]");
            var relate = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"block-related\")]");
            var tool = contentpage.ChildNodes.LastOrDefault(e => e.Name == "div");
            string auth = "";
            var author = doc.DocumentNode.SelectSingleNode("//p[contains(@class, \"author\")]");
            if (author != null)
            {
                auth = author.InnerText;
            }
            if (relate != null)
            {
                contentpage.RemoveChild(relate);
                
            }
            if (tool != null)
            {
                contentpage.RemoveChild(tool);
                
            }
            var des = "<i>" +  doc.DocumentNode.SelectSingleNode("//h2[contains(@class, \"summary\")]").InnerText + "</i>";
            var image = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"wrapper-img\")]").ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value;
            return new New
            (
                title,
                contentpage.InnerHtml,des, 
                "cuoi.tuoitre.vn-" + auth, image,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("//article[contains(@class, \"art-lastest\")]");
            List<string> links = new List<string>();
            for (int i = 0; i < articles.Count; i++)
            {
                var a = articles[i].ChildNodes.FirstOrDefault(e => e.Name == "a");
                links.Add("https://cuoi.tuoitre.vn/" + a.Attributes["href"].Value);
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