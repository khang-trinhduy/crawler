using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WordPressSharp;
using WordPressSharp.Models;

namespace Crawler.Models
{
    public class Wordpress
    {
        public Wordpress()
        {
        }
        public static void CreateAsync(PostCreateModel _post)
        {
            var config = new WordPressSiteConfig
            {
                Username = "crawler",
                Password = "PLkus6X%n5H!$j(E@$OzN@TD",
                BaseUrl = "https://www.saigontown.com/",
                BlogId = 1
            };

            using (var client = new WordPressClient(config))
            {
                List<Term> terms = new List<Term>();
                for (int i = 0; i < _post.tags.Count; i++)
                {
                    var nOC = _post.tags[i].Split(' ').Length;
                    if (nOC > 7)
                    {
                        continue;
                    }
                    var tag = _post.tags[i];
                    var exist = client.GetTerms("post_tag", new TermFilter
                    {
                        Search = tag
                    });
                    if (exist.Length > 0)
                    {
                        terms.Add(exist[0]);
                    }
                    else
                    {
                        var term = new Term
                        {
                            Name = tag,
                            Description = "term description",
                            Slug = String.Join('_', tag.ToLower().Split(" ")),
                            Taxonomy = "post_tag"
                        };
                        var termId = client.NewTerm(term);
                        term.Id = termId;
                        terms.Add(term);
                    }
                }

                for (int i = 0; i < _post.categories.Count; i++)
                {
                    terms.Add(new Term
                    {
                        Id = _post.categories[i],
                        Taxonomy = "category"
                    });
                }

                var post = new Post
                {
                    PostType = "post",
                    Title = _post.title,
                    Content = _post.content,
                    PublishDateTime = DateTime.Now,
                    Terms = terms.ToArray(),
                    Status = "publish",

                };
                string realImg = _post.img;
                if (!_post.Sggp)
                {
                    realImg = _post.img.Split("_")[0];
                    if (!realImg.Contains(".jpg") && !realImg.Contains(".jpeg"))
                    {
                        realImg += ".jpg";
                    }
                }
                // if (_post.Bds)
                // {
                //     client.NewPost(post);
                //     return;
                // }
                var featureImage = Data.CreateFromUrl(realImg);
                var media = client.UploadFile(featureImage);
                post.FeaturedImageId = media.Id;

                var id = Convert.ToInt32(client.NewPost(post));
            }

        }

    }
}