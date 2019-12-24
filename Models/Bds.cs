using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Bds : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            //covp: cao-oc-van-phong, tttm: trung-tam-thuong-mai, kdtm: khu-do-thi-moi, kph: khu-phuc-hop, noxh: nha-o-xa-hoi, 
            //kndst: khu-nghi-duong-sinh-thai, kcn: khu-cong-nghiep, dak: du-an-khac, btlk: biet-thu-lien-ke
            if (type == "chcc")
            {
                SetUrl("https://batdongsan.com.vn/can-ho-chung-cu");
                Type = "Căn hộ chung cư";
            }
            else if (type == "covp")
            {
                SetUrl("https://batdongsan.com.vn/cao-oc-van-phong");
                Type = "Cao ốc văn phòng";
            }
            else if (type == "tttm")
            {
                SetUrl("https://batdongsan.com.vn/trung-tam-thuong-mai");
                Type = "Trung tâm thương mại";
            }
            else if (type == "kdtm")
            {
                SetUrl("https://batdongsan.com.vn/khu-do-thi-moi");
                Type = "Khu đô thị mới";
            }
            else if (type == "kph")
            {
                SetUrl("https://batdongsan.com.vn/khu-phuc-hop");
                Type = "Khu phức hợp";
            }
            else if (type == "noxh")
            {
                SetUrl("https://batdongsan.com.vn/nha-o-xa-hoi");
                Type = "Nhà ở xã hội";
            }
            else if (type == "kndst")
            {
                SetUrl("https://batdongsan.com.vn/khu-nghi-duong-sinh-thai");
                Type = "Khu nghỉ dưỡng sinh thái";
            }
            else if (type == "kcn")
            {
                SetUrl("https://batdongsan.com.vn/khu-cong-nghiep");
                Type = "Khu công nghiệp";
            }
            else if (type == "dak")
            {
                SetUrl("https://batdongsan.com.vn/du-an-khac");
                Type = "Dự án khác";
            }
            else if (type == "btlk")
            {
                SetUrl("https://batdongsan.com.vn/biet-thu-lien-ke");
                Type = "Biệt thự liền kề";
            }
            else if (type == "ttt")
            {
                SetUrl("https://batdongsan.com.vn/tin-thi-truong");
                Type = "Tin thị trường";
            }
            return await GetNews(quantity);
        }
        private async Task<List<New>> GetNews(int quantity)
        {
            List<string> links = await GetLinks();
            List<New> news = new List<New>();
            for (int i = 0; i < quantity; i++)
            {
                var n = await GetContents("https://batdongsan.com.vn" + links[i]);
                n.Url = "https://batdongsan.com.vn" + links[i];
                news.Add(n);
            }
            return news;
        }

        private async Task<List<string>> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var articles = docs.DocumentNode.SelectNodes("/html/body/form/div[4]/div[8]/div/div[1]/div/ul/li/div[2]/div[1]/h3/a");
            List<string> links = new List<string>();
            foreach (var a in articles)
            {
                links.Add(a.Attributes["href"].Value);
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
            ////*[contains(@class, "prj-noidung")]/div/p
            var doc = await GetDocuments(url);
            string contents = "", rendered = "", description = "";
            var title = doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[6]/h1");
            // title = Decode(title);
            description = doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[6]/span[1]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[6]/span[1]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("/html/body/form/div[4]/div[8]/div[1]/p") != null ? doc.DocumentNode.SelectNodes("/html/body/form/div[4]/div[8]/div[1]/p") : null;
            paragraphs = paragraphs != null ? paragraphs : doc.DocumentNode.SelectNodes("//*[contains(@class, \"prj-noidung\")]/div/p");
            rendered = doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[8]/div[1]") != null ? doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[8]/div[1]").OuterHtml : string.Empty;
            var img = doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[6]/div[2]/div/div/div[1]/div/div[1]/div/img").Attributes["src"].Value;
            var general = doc.DocumentNode.SelectSingleNode("/html/body/form/div[4]/div[6]/div[2]/div/div/div[2]");
            var table = GetTable(general);
            if (paragraphs != null)
            {
                foreach (var para in paragraphs)
                {
                    contents += para != null ? Decode(para.InnerText) : string.Empty;
                }
                
            }
            return new New
            (
                title.InnerText,
                rendered,
                description,
                "batdongsan.com" + "-" + Type,
                img,
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