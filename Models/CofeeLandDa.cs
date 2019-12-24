using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class CofeeLandDa : Website
    {
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {

            SetUrl("https://cafeland.vn/du-an/");
            Type = "Dự án";
            this.Categories = "36-412";
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
                    n.Url = links[i].Link;
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
            var main = docs.DocumentNode.SelectSingleNode("/html/body/div[2]/div[2]/div[2]/div[1]/div[2]/ul/li/h3/a");
            var articles = docs.DocumentNode.SelectNodes("/html/body/div[2]/div[2]/div[2]/div[3]/div[1]/div[2]/ul/li/h3/a");
            List<LinkObj> links = new List<LinkObj>();
            try
            {
                links.Add(new LinkObj { Link = main.Attributes["href"].Value });
            }
            catch (System.Exception)
            {

            }
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(
                    new LinkObj
                    {
                        Link = articles[i].Attributes["href"].Value,
                        Title = articles[i].InnerText.Trim(),
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
            var img = doc.DocumentNode.SelectSingleNode("//*[@id=\"sevenBoxNewContentInfo\"]/div[2]/img");
            var title = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/h1");
            var contents = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/article");
            var auth = doc.DocumentNode.SelectSingleNode("/html/body/section[4]/div[2]/div[1]/div[1]/div/strong");
            return new New(
                title.InnerText, contents.InnerHtml,
                "", "ndh.vn" + "-" + Type + "-" + auth.InnerText
                , obj.Img, this.Categories
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