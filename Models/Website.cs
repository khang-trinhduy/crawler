using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Models
{
    public abstract class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public List<New> News { get; set; }
        public abstract string GetUrl();
        public abstract bool SetUrl(string url);
        public abstract Task<List<New>> GetTopNews(int quantity, string type);
        public abstract Task<string> Template(string url);
        public abstract New Normalize(New n);
        public string Categories { get; set; }
        
        public string RemoveLink(string content)
        {
            content = content.Trim();
            if (content == "")
            {
                return content;
            }
            else {
                content = content.Replace("<a", "<i");
                content = content.Replace("</a", "</i");
            }
            return content;
        }
    }
}