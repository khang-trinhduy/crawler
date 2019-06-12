using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using Crawler.Models;
using Newtonsoft;
using Newtonsoft.Json;
using Crawler.Data;

namespace Crawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpidersController : ControllerBase
    {
        private readonly CrawlerContext _context;

        public SpidersController(CrawlerContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")] 
        public async Task<ActionResult<IEnumerable<New>>> Get([FromQuery]string sort, [FromQuery]string category, [FromQuery]string search, [FromQuery]int page)
        {
            var news = _context.News.Where(e => true);
            if (SortById(sort))
            {
                if (SortByDescending(sort))
                {
                    news = news.OrderByDescending(n => n.Id);
                }
                else
                {
                    news = news.OrderBy(n => n.Id);
                }
            }
            else if (sort == "id-asce")
            {
                news = news.OrderBy(n => n.Id);
            }
            return Ok();
        }

        private static bool SortByDescending(string sort)
        {
            return sort.Split("-")[1] == "asce";
        }

        private static bool SortById(string sort)
        {
            return sort.Split("-")[0] == "id";
        }

        [HttpGet]
        [Route("topnews")]
        public async Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery]int quantity, [FromQuery]string subject, [FromQuery]string website = "vne")
        {
            if (quantity > 0)
            {
                if (website.Equals("vne"))
                {
                    Website vnexpress = new Vne();
                    vnexpress.SetUrl("https://vnexpress.net");
                    var news = await vnexpress.GetTopNews(quantity, subject);
                    if (news != null)
                    {
                        foreach (var n in news)
                        {
                            var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                            if (exist == null)
                            {
                                _context.News.Add(n);

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

                    return NotFound();
                }
                else if (website == "cafe" || website == "cafef" || website == "cofee")
                {
                    Website cofee = new Cofee();
                    var news = await cofee.GetTopNews(quantity, subject);
                    if (news != null)
                    {
                        foreach (var n in news)
                        {
                            var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                            if (exist == null)
                            {
                                _context.News.Add(n);

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

                    return NotFound();
                }
                else if (website == "bds" || website == "batdongsan")
                {
                    Website bds = new Bds();
                    var news = await bds.GetTopNews(quantity, subject);
                    if (news != null)
                    {
                        foreach (var n in news)
                        {
                            var exist = _context.News.FirstOrDefault(e => e.Url == n.Url && e.Title == n.Title);
                            if (exist == null)
                            {
                                _context.News.Add(n);

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

                    return NotFound();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("template/{id:int}")]
        public async Task<ActionResult<string>> Review(int id)
        {
            var n = await _context.News.FindAsync(id);
            if (n == null)
            {
                return NotFound();
            }

            Website web = new Vne();
            var template = await web.Template(n.Url);
            //NOTE open temp html file
            var tempFile = Path.GetTempPath() + "template.html";
            StreamWriter wt = new StreamWriter(tempFile);
            wt.Write(template);
            wt.Close();
            Process.Start(@"cmd.exe ", @"/c " + tempFile);
            return Ok(template);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
