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
    public class VneController : SpidersController
    {
        public VneController(CrawlerContext context, IConfiguration config) : base(context, config)
        {
        }

        public override async Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery] int quantity, [FromQuery] string subject)
        {
            if (quantity > 0)
            {
                Website cofee = new Vneco();
                var news = await cofee.GetTopNews(quantity, subject);
                if (news != null)
                {
                    foreach (var n in news)
                    {

                        try
                        {
                            var post = PostCreateModel.Create(n);
                            if (post == null)
                            {
                                continue;
                            }
                            Wordpress.CreateAsync(post);

                        }
                        catch (System.Exception e)
                        {
                            continue;
                        }

                    }

                    return Ok(news);
                }
            }
            return BadRequest(nameof(quantity));
        }
    }
}