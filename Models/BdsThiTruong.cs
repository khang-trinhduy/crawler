using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class BdsThiTruong : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            //covp: cao-oc-van-phong, tttm: trung-tam-thuong-mai, kdtm: khu-do-thi-moi, kph: khu-phuc-hop, noxh: nha-o-xa-hoi, 
            //kndst: khu-nghi-duong-sinh-thai, kcn: khu-cong-nghiep, dak: du-an-khac, btlk: biet-thu-lien-ke

            if (type == "ttt" || type == "")
            {
                SetUrl("https://batdongsan.com.vn/tin-thi-truong");
                Type = "Tin thị trường";
                this.Categories = "36-134";
            }
            return await GetNews(quantity);
        }
        private async Task<List<New>> GetNews(int quantity)
        {
            List<LinkObj> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                if (i < links.Count)
                {
                    var n = await GetContents("https://batdongsan.com.vn" + links[i].Link);
                    n.Url = "https://batdongsan.com.vn" + links[i].Link;
                    n.Title = links[i].Title;
                    news.Add(n);
                        
                }
            }
            return news;
        }

        private async Task<List<LinkObj>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("//*[@id=\"ctl23_BodyContainer\"]/div/div/h3/a");
            List<LinkObj> links = new List<LinkObj>();
            foreach (var a in articles)
            {
                links.Add(
                    new LinkObj
                    {
                        Link = a.Attributes["href"].Value,
                        Title = a.InnerText.Trim()
                    }
                    );
            }

            return links;
        }

        private async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Chrome/51.0.2704.103");
            var mainPage = await client.GetStringAsync(url);
            HtmlDocument docs = new HtmlDocument();
            docs.LoadHtml(mainPage);
            return docs;
        }

        private async Task<string> GetTemplate(string url)
        {
            var doc = await GetDocuments(url);
            return doc.DocumentNode.InnerHtml;
        }

        private async Task<New> GetContents(string url)
        {
            var doc = await GetDocuments(url);
            var title = doc.DocumentNode.SelectSingleNode("//*[@id=\"LeftMainContent\"]/div[1]/div/div[1]/h1");
            var contents = doc.DocumentNode.SelectSingleNode("//*[@id=\"divContents\"]");
            var trash = contents.ChildNodes.Where(e => e.Name == "p" && e.FirstChild != null && e.FirstChild.Name == "strong").LastOrDefault();
            if (trash != null && contents != null)
            {
                contents.RemoveChild(trash);
            }
            var table = contents.ChildNodes.Where(e => e.Name == "table" || e.Name == "ul").ToList();
            if (table != null && contents != null)
            {
                foreach (var item in table)
                {
                    contents.RemoveChild(item);
                }
            }
            if (contents != null)
            {
                foreach (var item in contents.ChildNodes)
                {
                    if (item.InnerHtml.Trim() != "")
                    {
                        if (item.InnerHtml.Contains("<img"))
                        {
                            continue;
                        }
                        item.InnerHtml = item.InnerHtml.Replace("batdongsan.com", "batdongsan", StringComparison.OrdinalIgnoreCase);
                        item.InnerHtml = item.InnerHtml.Replace("batdongsan.com.vn", "batdongsan", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            string auth = trash != null ? trash.InnerText : "";
            var img = contents.ChildNodes.Where(e => e.Name == "p" && e.FirstChild != null && e.FirstChild.Name == "img").FirstOrDefault();
            var image = img != null ? img.FirstChild.Attributes["src"].Value : "";
            return new New(
            
                title.InnerText,
                // Contents = rendered + table,
                this.RemoveLink(contents.InnerHtml),"",
                // Description = description,
                "batdongsan.com" + "-" + Type + "-" + auth,
                image,
                
                this.Categories
            );
        }

        private string GetTable(HtmlNode general)
        {
            var table = "";
            // remove trash
            var rows = general.ChildNodes.Where(e => e.Name != "#text").ToList();
            var header = rows[0];
            table += "<h2>" + header.InnerText + "</h2>";
            table += "<table><thead></thead><tbody>";
            for (int i = 1; i < rows.Count; i++)
            {
                HtmlNode row = rows[i];
                var cells = row.ChildNodes.Where(e => e.Name != "#text").ToList();
                if (cells.Count >= 2)
                {
                    table += "<tr><td>" + cells[0].InnerText + "</td><td>" + cells[1].InnerText + "</td></tr>";
                }
            }
            table += "</tbody></table>";
            return table;
        }
        //NOTE decode special html characters
        private string Decode(string strToDecode)
        {
            return System.Net.WebUtility.HtmlDecode(strToDecode);
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

        public override New Normalize(New n)
        {
            if (n == null)
            {
                throw new Exception(nameof(n));
            }
            n.Rendered = n.Rendered.Replace("<strong>", "<p>");
            n.Rendered = n.Rendered.Replace("</strong>", "</p>");
            n.Rendered = n.Rendered.Replace("</em>", "</p>");
            n.Rendered = n.Rendered.Replace("<em>", "<p>");
            return n;
        }

        
    }
}