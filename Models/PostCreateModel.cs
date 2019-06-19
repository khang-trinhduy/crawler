using System;
using System.Collections.Generic;

namespace Crawler.Models
{
    public class PostCreateModel
    {
        // public DateTime Date { get; set; }
        // public string Link { get; set; }
        // public string Slug { get; set; }
        // public string Type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        // public string Template { get; set; }
        // public List<int> Categories { get; set; }
        // public List<int> Tags { get; set; }

        public static PostCreateModel Create(New n)
        {
            return new PostCreateModel{
                title = n.Title.ToString(),
                content = n.Rendered.ToString()
            };
        }
    }
}