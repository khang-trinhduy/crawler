using System;
namespace Crawler.Models
{
    public class New
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Contents { get; set; }
        public string Author { get; set; }
        public DateTime Publish { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Rendered { get; set; }
    }
}