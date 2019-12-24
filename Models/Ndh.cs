using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Ndh : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ck" || type == "")
            {
                SetUrl("https://ndh.vn/chung-khoan");
                Type = "Chứng khoán";
                this.Categories = "34";
            }
            else if (type == "dn" )
            {
                SetUrl("https://ndh.vn/doanh-nghiep");
                Type = "Doanh nghiệp";
                this.Categories = "46";
            }
            else if (type == "tc")
            {
                SetUrl("https://ndh.vn/tai-chinh");
                Type = "Tài chính";
                this.Categories = "53";
            }
            else if (type == "bds")
            {
                SetUrl("https://ndh.vn/bat-dong-san");
                Type = "Bất động sản";
                this.Categories = "36";
            }
            else if (type == "hh")
            {
                SetUrl("https://ndh.vn/hang-hoa");
                Type = "Chứng khoán";
                this.Categories = "364";
            }
            else if (type == "vm")
            {
                SetUrl("https://ndh.vn/vi-mo");
                Type = "Vĩ mô";
                this.Categories = "239";
            }
            else if (type == "qt")
            {
                SetUrl("https://ndh.vn/quoc-te");
                Type = "Quốc tế";
                this.Categories = "365";
            }
            else if (type == "td")
            {
                SetUrl("https://ndh.vn/tieu-dung");
                Type = "Tiêu dùng";
                this.Categories = "366";
            }
            else if (type == "lg")
            {
                SetUrl("https://ndh.vn/lam-giau");
                Type = "Làm giàu";
                this.Categories = "367";
            }
            return await GetNews(quantity);
        }
        private async Task<List<New>> GetNews(int quantity)
        {
            List<LinkObj> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {   
                try
                {
                    var n = await GetContents(links[i]);
                    n.Url = "https://ndh.vn" + links[i].Link;
                    news.Add(n);
                    
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            return news;
        }

        private async Task<List<LinkObj>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var main = docs.DocumentNode.SelectSingleNode("/html/body/section[5]/div[1]/div[1]/article/div[1]/a");
            var mainImg = docs.DocumentNode.SelectSingleNode("/html/body/section[5]/div[1]/div[1]/article/div[1]/a/img").Attributes["src"].Value;
            var articles = docs.DocumentNode.SelectNodes("/html/body/section[5]/div[1]/div[2]/article/div/a");
            var articleImgs = docs.DocumentNode.SelectNodes("/html/body/section[5]/div[1]/div[2]/article/div/a/img");
            var childs = docs.DocumentNode.SelectNodes("/html/body/section[5]/div[1]/div[3]/article/div[1]/a");
            var childImgs = docs.DocumentNode.SelectNodes("/html/body/section[5]/div[1]/div[3]/article/div[1]/a/img");
            List<LinkObj> links = new List<LinkObj>();
            try
            {
                links.Add(new LinkObj { Link = "https://ndh.vn" + main.Attributes["href"].Value, Img = mainImg });
            }
            catch (System.Exception)
            {

            }
            for (int i = 0; i < articles.Count; i++)
            {
                var img = articleImgs[i].Attributes["src"].Value;
                img = img.Split("_")[0];
                img = img.Contains(".jpg") ? img : img + ".jpg";
                links.Add(
                    new LinkObj
                    {
                        Link = "https://ndh.vn" + articles[i].Attributes["href"].Value,
                        Title = articles[i].InnerText.Trim(),
                        Img = img
                    }
                    );
            }

            for (int i = 0; i < childs.Count; i++)
            {
                var img = childImgs[i].Attributes["src"].Value;
                img = img.Split("_")[0];
                img = img.Contains(".jpg") ? img : img + ".jpg";
                links.Add(
                    new LinkObj
                    {
                        Link = "https://ndh.vn" + childs[i].Attributes["href"].Value,
                        Title = childs[i].InnerText.Trim(),
                        Img = img
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

        private async Task<New> GetContents(LinkObj obj)
        {
            var doc = await GetDocuments(obj.Link);
            var title = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/h1");
            var contents = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/article");
            var auth = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/div[1]/div[1]/div/strong");
            return new New(title.InnerText, contents.InnerHtml, "", "ndh.vn" + "-" + Type + "-" + auth.InnerText, obj.Img, this.Categories);
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