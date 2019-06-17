using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Models
{
    public class Vneco : Website
    {
        public override Task<List<New>> GetTopNews(int quantity, string type)
        {
            throw new System.NotImplementedException();
        }

        public override string GetUrl()
        {
            throw new System.NotImplementedException();
        }

        public override bool SetUrl(string url)
        {
            throw new System.NotImplementedException();
        }

        public override Task<string> Template(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}