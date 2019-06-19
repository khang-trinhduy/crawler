using System;
using System.Collections.Generic;
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
            }
            else if (type == "covp")
            {
                SetUrl("https://batdongsan.com.vn/cao-oc-van-phong");
            }
            else if (type == "tttm")
            {
                SetUrl("https://batdongsan.com.vn/trung-tam-thuong-mai");
            }
            else if (type == "kdtm")
            {
                SetUrl("https://batdongsan.com.vn/khu-do-thi-moi");
            }
            else if (type == "kph")
            {
                SetUrl("https://batdongsan.com.vn/khu-phuc-hop");
            }
            else if (type == "noxh")
            {
                SetUrl("https://batdongsan.com.vn/nha-o-xa-hoi");
            }
            else if (type == "kndst")
            {
                SetUrl("https://batdongsan.com.vn/khu-nghi-duong-sinh-thai");
            }
            else if (type == "kcn")
            {
                SetUrl("https://batdongsan.com.vn/khu-cong-nghiep");
            }
            else if (type == "dak")
            {
                SetUrl("https://batdongsan.com.vn/du-an-khac");
            }
            else if (type == "btlk")
            {
                SetUrl("https://batdongsan.com.vn/biet-thu-lien-ke");
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
            var articles = docs.DocumentNode.SelectNodes("//*[@id=\"form1\"]/div[4]/div[8]/div/div[1]/div/ul/li/div[2]/div[1]/h3/a");
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
            string title = "", contents = "", rendered = "";
            title = doc.DocumentNode.SelectSingleNode("//*[contains(@class, \"prj-noidung\")]/div/h2") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, \"prj-noidung\")]/div/h2").OuterHtml : null;
            title = title != null ? title : doc.DocumentNode.SelectSingleNode("//*[contains(@class, \"prj-noidung\")]/h2").OuterHtml;
            title = Decode(title);
            // description = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]") != null ? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'description')]").InnerText : string.Empty;
            var paragraphs = doc.DocumentNode.SelectNodes("//*[contains(@class, \"prj-noidung\")]/p") != null ? doc.DocumentNode.SelectNodes("//*[contains(@class, \"prj-noidung\")]/p") : null;
            paragraphs = paragraphs != null ? paragraphs : doc.DocumentNode.SelectNodes("//*[contains(@class, \"prj-noidung\")]/div/p");
            rendered = doc.DocumentNode.SelectSingleNode("//*[@id=\"form1\"]/div[4]/div[8]/div[1]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"form1\"]/div[4]/div[8]/div[1]").OuterHtml : string.Empty;
            foreach (var para in paragraphs)
            {
                contents += para != null ? Decode(para.InnerText) : string.Empty;
            }
            return new New
            {
                Title = title,
                Contents = contents,
                Rendered = rendered
            };
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
    }
}