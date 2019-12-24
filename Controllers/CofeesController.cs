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
    public class CofeesController : SpidersController
    {
        public CofeesController(CrawlerContext context, IConfiguration config) : base(context, config)
        {
        }

        public override async Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery] int quantity, [FromQuery] string subject)
        {
            if (quantity > 0)
            {
                var result = new Result();
                Website cofee = new Cofee();
                var news = await cofee.GetTopNews(quantity, subject);
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
                    }
                    await _context.SaveChangesAsync();

                    return Ok(result);
                }
            }
            return BadRequest(nameof(quantity));
        }
    }
}