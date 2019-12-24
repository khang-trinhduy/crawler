// /html/body/form/div[3]/div/div/div/div[1]/div/div/div/div/div[1]/div/div[2]/h2/a
///html/body/div[10]/div[3]/div/div[1]/div[1]/div[1]/h1
using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crawler.Models
{
    public class NewPolicy
    {
        public bool Updated { get; set; }
        public string Link { get; set; }
    }
    public class Hcmc
    {
        public string Url { get; set; } = "http://congbao.hochiminhcity.gov.vn/cong-bao/van-ban/linh-vuc/dat-dai-xay-dung/25";

        public async Task<NewPolicy> GetNews()
        {
            var link =await this.GetLinks();
            if (link != "")
            {
                return new NewPolicy {
                    Updated = true, Link = link
                };
            }else {
                return new NewPolicy{
                    Updated = false, Link = ""
                };
            }
        }

        public async Task<string> GetLinks()
        {
            HtmlDocument docs = await GetDocuments(this.Url);
            var date = docs.DocumentNode.SelectSingleNode("/html/body/div[1]/div[3]/div/div[2]/div[2]/div/div/div[2]/div/table/tbody/tr[1]/td[1]");
            var link = docs.DocumentNode.SelectSingleNode("/html/body/div[1]/div[3]/div/div[2]/div[2]/div/div/div[2]/div/table/tbody/tr[1]/td[2]/a");
            string str = "";
            if (date != null)
            {
                DateTime tmp;
                var day = DateTime.TryParse(date.InnerText, out tmp);
                if (tmp.Day >= DateTime.Now.Day)
                {
                    if (link != null)
                    {
                        str = "http://congbao.hochiminhcity.gov.vn" +  link.Attributes["href"].Value;  
                    }
                }
            }
            return str;
        }

        public async Task<HtmlDocument> GetDocuments(string url)
        {
            HttpClient client = new HttpClient();
            // client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
            var mainPage = await client.GetStringAsync(url);
            HtmlDocument docs = new HtmlDocument();
            docs.LoadHtml(mainPage);
            return docs;
        }
     
    }
}