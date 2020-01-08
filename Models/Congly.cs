using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Congly : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ta" || type == "")
            {
                SetUrl("https://conglyxahoi.net.vn/toa-an/");
                this.Categories = "2419-2372";
            }
            else if (type == "xx")
            {
                SetUrl("https://conglyxahoi.net.vn/toa-an/cau-chuyen-phap-dinh/");
                this.Categories = "2418-2372";
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
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"ct-chitiet\")]");
            var des = "<i>" +  doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"sapo-chitiet\")]").InnerText + "</i>";
            var title = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, \"tit-chitiet\")]").InnerText;
            var relates = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"news-related\")]");
            contentpage.RemoveChild(relates);
            string author = "conglyxahoi.net.vn";
            return new New
            (
                title,
                des + contentpage.InnerHtml,des, 
                author, img,this.Categories
            );
        }
        private async Task<List<LinkObj>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var rank1 = docs.DocumentNode.SelectSingleNode("//div[contains(@class, \"pic-tinnoibat\")]").ChildNodes.FirstOrDefault(e => e.Name == "a");
            var rank2 = docs.DocumentNode.SelectNodes("/html/body/div[9]/div[3]/div[1]/div[1]/div[2]/div[2]/div/div/div[1]/a");
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[9]/div[3]/div[3]/div[1]/div[1]/div/div/a");
            List<LinkObj> links = new List<LinkObj>();
            links.Add(new LinkObj{Link = rank1.Attributes["href"].Value, Img = rank1.ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value});
            for (int i = 0; i < rank2.Count; i++)
            {
                links.Add(new LinkObj{Link = rank2[i].Attributes["href"].Value, Img = rank2[i].ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value});
            }
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(new LinkObj{Link = articles[i].Attributes["href"].Value, Img = articles[i].ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value});
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