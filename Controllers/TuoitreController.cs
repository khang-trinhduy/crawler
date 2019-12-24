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
    public class TuoitreController : SpidersController
    {
        public TuoitreController(CrawlerContext context, IConfiguration config) : base(context, config)
        {
        }

        public async override Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery] int quantity, [FromQuery] string subject)
        {
            if (quantity > 0)
            {
                Website bds = new Tuoitre();
                var news = await bds.GetTopNews(quantity, subject);
                if (news != null)
                {
                    foreach (var n in news)
                    {
                        var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                        // if (exist == null)
                        // {
                        // _context.News.Add(n);
                        try
                        {
                            var post = PostCreateModel.Create(n);
                            if (post == null)
                            {
                                // continue;
                            }
                            await CreatePost(post);

                        }
                        catch (System.Exception e)
                        {
                            // continue;
                            return BadRequest(e.Message);
                        }
                        // }
                    }
                    // try
                    // {
                    //     await _context.SaveChangesAsync();
                    // }
                    // catch (Exception e)
                    // {

                    //     throw new Exception(e.Message);
                    // }
                    return Ok(news);
                }

                return NotFound();
            }
            return BadRequest(nameof(quantity));
        }
    }
}