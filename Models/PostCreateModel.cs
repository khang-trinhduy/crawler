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
        public string category { get; set; }
        public List<int> categories { get; set; } = new List<int>();
        public List<string> tags { get; set; } = new List<string>();

        public static PostCreateModel Create(New n)
        {
            PostCreateModel item = new PostCreateModel();
            item.tags = new List<string>();
            if (n.Tags.Trim() != string.Empty)
            {
                foreach (var tag in n.Tags.Trim().Split("%"))
                {
                    item.tags.Add(tag);
                }
            }
            item.category = n.Categories;
            item.title = n.Title.ToString();
            item.content = n.Contents.ToString();
            return item;
        }
    }
}