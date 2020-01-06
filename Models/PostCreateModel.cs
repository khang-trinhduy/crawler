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
        public bool Sggp { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        // public string Template { get; set; }
        public string category { get; set; }
        public List<string> categories { get; set; } = new List<string>();
        public List<string> tags { get; set; } = new List<string>();
        public string img { get; set; }
        public bool Bds { get; set; }

        public static PostCreateModel Create(New n)
        {
            PostCreateModel item = new PostCreateModel();
            item.tags = new List<string>();
            if (n.Tags != null && n.Tags.Trim() != string.Empty)
            {
                var taglist = n.Tags.Trim().Split('-');
                foreach (var tag in taglist)
                {
                    if (tag.Trim() == "www.sggp.org.vn" || tag.Trim() == "cafef.vn" || tag.Trim() == "vneconomy.vn" || tag.Trim() == "cuoi.tuoitre.vn" || tag.Trim() == "tuoitre.vn")
                    {
                        item.Sggp = true;
                    }
                    else if (tag.Trim() == "batdongsan.com" || tag.Trim() == "fackcheck.vn")
                    {
                        item.Bds = true;
                    }
                    if (tag.Trim() != "")
                    {
                        item.tags.Add(tag.Trim());
                    }
                }
            }
            var cats = n.Categories.Split("-");
            item.category = n.Categories;
            item.categories = new List<string>();
            for (int i = 0; i < cats.Length; i++)
            {
                if (cats[i].Trim() != "")
                {
                    item.categories.Add(cats[i]);
                }
            }
            item.title = n.Title.ToString();
            item.content = n.Rendered.ToString();
            item.img = n.Image;
            return item;
        }
    }
}