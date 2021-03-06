using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Nhadat : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "nd" || type == "")
            {
                SetUrl("https://www.muabannhadat.vn/tin-tuc/thong-tin-nha-dat");
                this.Categories = "134";
            }
            else if (type == "pt")
            {
                SetUrl("https://www.muabannhadat.vn/tin-tuc/blog-phong-thuy");
                this.Categories = "2005";
            }
            else if (type == "kt")
            {
                SetUrl("https://www.muabannhadat.vn/tin-tuc/thiet-ke-kien-truc");
                this.Categories = "398";
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
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"_mbndDetailPage\")]");
            title = contentpage.ChildNodes.Where(e => e.Name == "h1").FirstOrDefault().InnerText;
            var trashy = doc.DocumentNode.SelectSingleNode("//span[contains(@class, \"_dateTime\")]");
            var other = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"mbnd_detail_descripton\")]");
            var btm = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"__DesFull\")]");
            contentpage.RemoveChild(btm);
            var author = other.ChildNodes.LastOrDefault(e => e.HasClass("pull-right"));
            if (author != null)
            {
                other.RemoveChild(author);
                
            }
            contentpage.RemoveChild(trashy);
            contentpage.AppendChild(other);
            var des = "<i>" +  doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"_DesSummary\")]").InnerText + "</i>";
            var auth = "MuaBanNhaDat";
            var image = doc.DocumentNode.SelectSingleNode("//img[contains(@class, \"img-thumbnail\")]").Attributes["src"].Value;
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
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[4]/div/div[1]/div[1]/div/div/a[1]");
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