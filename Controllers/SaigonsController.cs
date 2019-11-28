using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crawler.Data;
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
                Website saigon = new Saigon();
                saigon.SetUrl("https://www.sggp.org.vn");
                var news = await saigon.GetTopNews(quantity, subject);
                if (news != null)
                {
                    foreach (var n in news)
                    {
                        var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                        if (exist == null)
                        {
                            _context.News.Add(n);

                            try
                            {
                                var post = PostCreateModel.Create(n);
                                if (post == null)
                                {
                                    return BadRequest();
                                }
                                await CreatePost(post);

                            }
                            catch (System.Exception e)
                            {
                                return BadRequest(e.Message);
                            }

                        }
                    }
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {

                        throw new Exception(e.Message);
                    }
                    return Ok(news);
                }
            }
            return BadRequest(nameof(quantity));
        }
    }
}