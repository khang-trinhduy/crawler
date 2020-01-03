using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;

namespace Crawler.Models
{
    public class Vneco : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ts")
            {
                SetUrl("http://vneconomy.vn/timeline/9920/trang-1.htm");
                this.Categories = "110-139";
            }
            else if (type == "tc")
            {
                SetUrl("http://vneconomy.vn/timeline/6/trang-1.htm");
                this.Categories = "53";
            }
            else if (type == "ck" || type == "")
            {
                SetUrl("http://vneconomy.vn/timeline/7/trang-1.htm");
                this.Categories = "34";
            }
            else if (type == "dn")
            {
                SetUrl("http://vneconomy.vn/timeline/5/trang-1.htm");
                this.Categories = "46";
            }
            else if (type == "do")
            {
                SetUrl("http://vneconomy.vn/timeline/17/trang-1.htm");
                this.Categories = "64";
            }
            else if (type == "tt")
            {
                SetUrl("http://vneconomy.vn/timeline/19/trang-1.htm");
                this.Categories = "64";
            }
            else if (type == "tg")
            {
                SetUrl("http://vneconomy.vn/timeline/99/trang-1.htm");
                this.Categories = "33";
            }
            else if (type == "css")
            {
                SetUrl("http://vneconomy.vn/timeline/16/trang-1.htm");
            }
            else if (type == "x3")
            {
                SetUrl("http://vneconomy.vn/timeline/23/trang-1.htm");
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
                    var n = await GetContents("http://vneconomy.vn/" + links[i]);
                    n.Url = "http://vneconomy.vn/" + links[i];
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
            string title = "", description = "", source = string.Empty;
            var contentpage = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"contentleft\")]");
            title = contentpage.ChildNodes.Where(e => e.Name == "h1").FirstOrDefault().InnerText;
            var trashy = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"sharemxhtop\")]");
            var des = "<i>" +  doc.DocumentNode.SelectSingleNode("//h2[contains(@class, \"sapo\")]").InnerText + "</i>";
            var boxcontent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"boxcontent\")]");
            description = "<i>" + contentpage.ChildNodes.Where(e => e.Name == "h2").FirstOrDefault().InnerText + "</i>";
            var auth = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"author\")]");
            boxcontent.RemoveChild(auth);
            var image = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"imgdetail\")]");
            var img = image.ChildNodes.FirstOrDefault(e => e.Name == "img").Attributes["src"].Value;
            return new New
            (
                title,
                des + image.InnerHtml + boxcontent.InnerHtml,des, 
                "vneconomy.vn-" + auth.InnerText, img,this.Categories
            );
        }
        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var li = docs.DocumentNode.ChildNodes.Where(e => e.Name == "li").ToList();
            List<string> links = new List<string>();
            for (int i = 0; i < li.Count; i++)
            {
                var info = li[i].ChildNodes.FirstOrDefault(e => e.HasClass("infonews"));
                if (info != null)
                {
                    var a = info.ChildNodes.FirstOrDefault(e => e.Name == "a");
                    if (a != null)
                    {
                        links.Add(a.Attributes["href"].Value);
                    }
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