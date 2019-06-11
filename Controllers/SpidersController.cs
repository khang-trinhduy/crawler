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

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var url = "https://vnexpress.net/";
            var client = new HttpClient();
            var contents = await client.GetStringAsync(url);
            var document = new HtmlDocument();
            document.LoadHtml(contents);
            var nodes = document.DocumentNode.SelectNodes("//*[@id=\"main_menu\"]/a").ToList();
            List<string> links = new List<string>();
            foreach (var node in nodes)
            {
                links.Add(node.Attributes["href"].Value);
            }
            var tempFile = Path.GetTempPath() + "template.html";
            // StreamWriter wt = new StreamWriter(tempFile);
            // wt.Write(document.DocumentNode.InnerHtml);
            // wt.Close();
            // Process.Start(@"cmd.exe ",@"/c " + tempFile);
            // var nav = document.DocumentNode.SelectNodes("//*[@id=\"main_menu\"]/a");
            
            return Ok(links);
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
            Process.Start(@"cmd.exe ",@"/c " + tempFile);
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
