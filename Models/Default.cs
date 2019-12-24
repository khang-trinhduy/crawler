using System;
using System.Collections.Generic;

namespace Crawler.Models
{
    public class Default
    {
        public Default(){}
        public Default(string rendered, string category)
        {
                if (this.IsRelevent(rendered, 5))
                {
                    this.Categories = "1050-" + category;
                }else
                {
                    this.Categories = category;
                }

        }
        public string Categories { get; set; }
        public string Rendered { get; set; }

        public Boolean IsRelevent(string articles, int threshold) 
        {
            var normalizedArticle = NormalizeString(articles);
            return GetNumberOfOccurences(normalizedArticle) > threshold;
        }

        public string NormalizeString(string str) {
            var lowerCased = str.ToLower();
            return lowerCased;
        }

        public int GetNumberOfOccurences(string str)
        {
            var total = 0;
            var keywords = ReleventKeyWords();
            for (int i = 0; i < keywords.Count; i++)
            {
                var lowered = keywords[i].ToLower();
                if (str.IndexOf(lowered) > -1)
                {
                    total++;
                }
            }
            return total;
        }
        public List<string> ReleventKeyWords() =>new List<string> {
                "HCM",
                "Sài Gòn",
                "Thành phố Hồ Chí Minh",
                "tp.hcm",
                "tp hcm",
                "Hồ Chí Minh",
                "sai gon",
                "sg",
                "Quận 1",
                "Quận 2",
                "Quận 3",
                "Quận 4",
                "Quận 5",
                "Quận 6",
                "Quận 7",
                "Quận 9",
                "Quận 8",
                "Quận 10",
                "Quận 11",
                "Quận 12",
                "Quận Bình Tân",
                "Quận Bình Thạnh",
                "Quận Bình Chánh",
                "Huyện Cần Giờ",
                "Huyện Củ Chi",
                "Huyện Hóc Môn",
                "Huyện Nhà Bè",
                "Quận Thủ Đức",
                "Quận Tân Phú",
                "Quận Tân Bình",
                "Quận Phú Nhuận",
                "Quận Gò Vấp",
        };
    }
}