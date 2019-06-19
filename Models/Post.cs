using System;
using System.Collections.Generic;

namespace Crawler.Models
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
        public string Slug { get; set; }
        public string Type { get; set; }
        public WpObject Title { get; set; }
        public WpObject Content { get; set; }
        public string Template { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Tags { get; set; }
        public Post()
        {
            
        }
        
    }

    public class WpObject
    {
        public string Rendered { get; set; }
        public Boolean Protected { get; set; }
    }
}