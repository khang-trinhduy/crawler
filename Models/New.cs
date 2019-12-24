using System;
using System.Collections.Generic;

namespace Crawler.Models
{
    public class New : Default
    {
        public New(){
            
        }
        public New(string title, string rendered, string des, string tag, string img, string category) : base(rendered, category)
        {
            this.Title = title;
            this.Rendered = rendered;
            this.Description = des;
            this.Image = img;
            this.Tags = tag;
        }
        public string Tags { get; set; }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Contents { get; set; }
        public string Author { get; set; }
        public DateTime Publish { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

       
    }
}