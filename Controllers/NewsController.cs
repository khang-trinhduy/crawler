using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crawler.DataContext;
using Crawler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Controllers
{
    [Route("api/[Controller]")]
    public class NewsController : ControllerBase
    {
        private readonly CrawlerContext _context;
        public NewsController(CrawlerContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<New>>> Get([FromQuery]string sort, [FromQuery]string search, [FromQuery]string filter, [FromQuery]int page)
        {
            var news = _context.News.Where(e => true);
            if (NewMethod(sort))
            {

            }
            return Ok();
        }

        private static bool NewMethod(string sort)
        {
            return sort.Split("-")[0] == "id";
        }
    }
}