using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crawler.DataContext;
using Crawler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Crawler.Controllers
{
    public class CofeeLandsController : SpidersController
    {
        public CofeeLandsController(CrawlerContext context, IConfiguration config) : base(context, config)
        {
        }

        public async override Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery] int quantity, [FromQuery] string subject)
        {
            var result = new Result();
            if (quantity > 0)
            {
                Website bds = new CofeeLand();
                if (subject == "da")
                {
                    bds = new CofeeLandDa();
                }
                else if (subject == "bds")
                {
                    bds = new CofeeLandBds();
                }
                var news = await bds.GetTopNews(quantity, subject);
                if (news != null)
                {
                    foreach (var n in news)
                    {
                        result.Total++;
                        var existed = _context.News.FirstOrDefault(e => e.Title.ToLower() == n.Title);
                        if (existed != null)
                        {
                            continue;
                        }
                        try
                        {
                            var post = PostCreateModel.Create(n);
                            if (post == null)
                            {
                                continue;
                            }
                            Wordpress.CreateAsync(post);
                            _context.News.Add(n);
                            result.Published++;

                        }
                        catch (System.Exception e)
                        {
                            continue;
                        }
                        // }
                    }
                    await _context.SaveChangesAsync();
                    return Ok(result);
                }

                return NotFound();
            }
            return BadRequest(nameof(quantity));
        }
    }
}