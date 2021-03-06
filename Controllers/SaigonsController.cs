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
    [ApiController]
    public class SaigonsController : SpidersController
    {
        public SaigonsController(CrawlerContext context, IConfiguration config) : base(context, config)
        {
        }
        public override async Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery] int quantity, [FromQuery] string subject)
        {
            if (quantity > 0)
            {
                var result = new Result();
                Website saigon = new Saigon();
                saigon.SetUrl("https://www.sggp.org.vn");
                var news = await saigon.GetTopNews(quantity, subject);
                if (news != null)
                {
                    foreach (var n in news)
                    {
                        result.Total++;
                        var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                        if (exist == null)
                        {

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
                    }

                    await _context.SaveChangesAsync();
                    return Ok(result);
                }
            }
            return BadRequest(nameof(quantity));
        }
    }
}