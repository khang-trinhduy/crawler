using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Models
{
    public class Cofee : Website
    {
        // 'http://cafef.vn/timeline/31/trang-1551.chn', ck
        // 'http://cafef.vn/timeline/112/trang-1551.chn', ts
        // 'http://cafef.vn/timeline/35/trang-1551.chn', bds
        // 'http://cafef.vn/timeline/36/trang-1.chn', dn
        // 'http://cafef.vn/timeline/34/trang-1.chn', nh
        // 'http://cafef.vn/timeline/32/trang-1.chn', tcqt
        // 'http://cafef.vn/timeline/33/trang-1.chn', vm
        // 'http://cafef.vn/timeline/114/trang-1.chn', s
        // 'http://cafef.vn/timeline/39/trang-1.chn', tt
        // 12
        public override async Task<List<New>> GetTopNews(int quantity, string type)
        {
            if (type == "ck")
            {
                SetUrl("http://cafef.vn/timeline/31/trang-1.chn");
                Type = "Chứng khoán";
                this.Categories = "34";
            }
            else if (type == "ts")
            {
                SetUrl("http://cafef.vn/timeline/112/trang-1.chn");
                Type = "Thời sự";
                this.Categories = "110";
            }
            else if (type == "bds")
            {
                SetUrl("http://cafef.vn/timeline/35/trang-1.chn");
                Type = "Bất động sản";
                this.Categories = "36";
            }
            else if (type == "dn")
            {
                SetUrl("http://cafef.vn/timeline/36/trang-1.chn");
                Type = "Doanh nhân";
                this.Categories = "46";
            }
            else if (type == "nh")
            {
                SetUrl("http://cafef.vn/timeline/34/trang-1.chn");
                Type = "Ngân hàng";
                this.Categories = "37";
            }
            else if (type == "tcqt")
            {
                SetUrl("http://cafef.vn/timeline/32/trang-1.chn");
                Type = "Tài chính quốc tế";
                this.Categories = "53";
            }
            else if (type == "vm")
            {
                SetUrl("http://cafef.vn/timeline/33/trang-1.chn");
                Type = "Vĩ mô";
                this.Categories = "239";
            }
            else if (type == "s")
            {
                SetUrl("http://cafef.vn/timeline/114/trang-1.chn");
                this.Categories = "2005";
            }
            else if (type == "tt")
            {
                SetUrl("http://cafef.vn/timeline/39/trang-1.chn");
                Type = "Thị trường";
                this.Categories = "64";
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
                    var n = await GetContents("http://cafef.vn/" + links[i].Link, links[i].Img);
                    n.Url = "http://cafef.vn/" + links[i];
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
            var articles = docs.DocumentNode.SelectNodes("/li/h3/a");
            var images = docs.DocumentNode.SelectNodes("/li/a/img");
            List<LinkObj> links = new List<LinkObj>();
            for (int i = 0; i < articles.Count; i++)
            {
                links.Add(new LinkObj
                {
                    Link = articles[i].Attributes["href"].Value,
                    Img = images[i].Attributes["src"].Value
                });
            }

            return links;
        }
        private async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            var mainPage = await client.GetStringAsync(url);
            HtmlDocument docs = new HtmlDocument();
            docs.LoadHtml(mainPage);
            return docs;
        }
        private async Task<New> GetContents(string url, string img)
        {
            var doc = await GetDocuments(url);
            string title = "", author = "", source = string.Empty;
            title = doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"left_cate\")]/*[contains(@class, \"title\")]") != null ? doc.DocumentNode.SelectSingleNode("//div[contains(@class, \"left_cate\")]/*[contains(@class, \"title\")]").InnerText : string.Empty;
            // author = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[1]").InnerText : string.Empty;
            source = doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]") != null ? doc.DocumentNode.SelectSingleNode("//*[@id=\"contentdetail\"]/p[2]").InnerText : string.Empty;
            var rendered = doc.DocumentNode.SelectSingleNode("/html/body/form/div[2]/div[4]/div[5]/div/div[1]/div[4]").ChildNodes.Count > 10
                ? doc.DocumentNode.SelectSingleNode("/html/body/form/div[2]/div[4]/div[5]/div/div[1]/div[4]") :
                doc.DocumentNode.SelectSingleNode("/html/body/form/div[2]/div[4]/div[5]/div/div[1]/div[5]");
            var trash = rendered.ChildNodes.Where(e => e.Name == "#text").ToList();
            foreach (var item in trash)
            {
                rendered.RemoveChild(item);
            }
            var news = rendered.ChildNodes.Where(e => e.Id == "contentdetail").FirstOrDefault();
            if (news != null)
            {
                var entrepneral = news.ChildNodes.Where(e => e.Name == "div").ToList();
                foreach (var item in entrepneral)
                {
                    news.RemoveChild(item);
                }
                var content = news.ChildNodes.Where(e => e.Name == "span").FirstOrDefault();
                if (content != null)
                {
                    var affiliate = content.ChildNodes.Where(e => e.Name == "div").LastOrDefault();
                    if (affiliate != null)
                    {
                        content.RemoveChild(affiliate);
                    }
                    var realSource = content.ChildNodes.Where(e => e.Name == "p" && e.InnerText.Trim() != "" && e.InnerText.Split(" ").Length < 5).LastOrDefault();
                    if (realSource != null)
                    {
                        content.RemoveChild(realSource);
                        author += realSource.InnerText.Trim();
                    }
                }
                var auths = news.ChildNodes.Where(e => e.Name == "p").ToList();
                for (int i = 0; i < auths.Count; i++)
                {
                    author += "-" + auths[i].InnerText.Trim();
                    news.RemoveChild(auths[i]);
                }
                var texts = news.ChildNodes.Where(e => e.Name == "#text").ToList();
                for (int i = 0; i < texts.Count; i++)
                {
                    news.RemoveChild(texts[i]);
                }
            }
            var ul = rendered.ChildNodes.Where(e => e.Name == "ul").FirstOrDefault();
            if (ul != null)
            {
                rendered.RemoveChild(ul);
            }
            var src = rendered.ChildNodes.Where(e => e.Id == "urlSourceCafeF").FirstOrDefault();
            if (src != null)
            {
                rendered.RemoveChild(src);
            }
            var scripts = rendered.ChildNodes.Where(e => e.Name == "script").ToList();
            for (int j = 0; j < scripts.Count; j++)
            {
                rendered.RemoveChild(scripts[j]);
            }
            var final = rendered.ChildNodes.Where(e => e.Name != "h2" && e.Id != "contentdetail").ToList();
            for (int i = 1; i < final.Count; i++)
            {
                rendered.RemoveChild(final[i]);
            }
            // var arti = rendered.ChildNodes[13];
            // var trash = arti.ChildNodes[1];
            // arti.RemoveChild(trash);
            var imgUrl = rendered.FirstChild.ChildNodes.Where(e => e.Name == "img").FirstOrDefault();
            if (imgUrl != null)
            {
                img = imgUrl.Attributes["src"].Value;
            }
            return new New
            (
                title,
                rendered.OuterHtml,"",
                "cafef.vn" + "-" + author,
                img, this.Categories
            );
        }

        public override string GetUrl()
        {
            return this.Url;
        }
        private async Task<string> GetTemplate(string url)
        {
            var doc = await GetDocuments(url);
            return doc.DocumentNode.InnerHtml;
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
            //NOTE remove tag
            var links = new Regex("(<a).+\n*\t*.+(</a>)");
            var matched = links.Matches(n.Rendered);
            if (matched != null && matched.Count > 0)
            {
                for (int i = 0; i < matched.Count; i++)
                {
                    n.Rendered = n.Rendered.Replace(matched[i].Value, "");
                }
            }
            var source = new Regex("(<p class=\"source).+\n*\t*.+(</p>)");
            var author = new Regex("(<p class=\"author).+\n*\t*.+(</p>)");
            var tags = new Regex("(<p class=\"author).+\n*\t*.+(</p>)");
            n.Rendered = Remove(source, n.Rendered);
            n.Rendered = Remove(author, n.Rendered);
            n.Rendered = n.Rendered.Replace("<b>", "<p>");
            n.Rendered = n.Rendered.Replace("<strong>", "<p>");
            n.Rendered = n.Rendered.Replace("</b>", "</p>");
            n.Rendered = n.Rendered.Replace("</strong>", "</p>");
            n.Rendered = n.Rendered.Replace("<p>Từ Khóa:</p>", "");
            return n;
        }
        private string Remove(Regex re, string str)
        {
            var matched = re.Match(str);
            if (matched.Success)
            {
                str = str.Replace(matched.Value, "");
            }
            return str;
        }
    }
}