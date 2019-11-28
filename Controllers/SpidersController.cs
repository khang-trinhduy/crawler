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
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Crawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class SpidersController : ControllerBase
    {
        protected readonly CrawlerContext _context;
        protected readonly IConfiguration _config;

        public SpidersController(CrawlerContext context, IConfiguration config)
        {
            _config = config;
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
        public abstract Task<ActionResult<IEnumerable<New>>> GetTopNews([FromQuery]int quantity, [FromQuery]string subject);
        protected async Task CreatePost(PostCreateModel post)
        {
            JWTAuth auth = new JWTAuth(_config["wpuri"]);
            auth.User = new JWTUser {
                UserName = _config["wpuser"],
                Password = _config["wppw"],
            };
            var httpResponseMsg = await auth.RequestJWToken();
            var content = httpResponseMsg.Content.ReadAsStringAsync().Result;
            var response = JsonConvert.DeserializeObject<JWTResponse>(content);
            auth.Token = response.Token;
            if (await auth.IsValidJWToken())
            {
                // var cats = await auth.GetCategories();
                // var catItems = JsonConvert.DeserializeObject<List<Category>>(await cats.Content.ReadAsStringAsync());
                // if (catItems.FirstOrDefault(e => string.Equals(e.Name, post.category, StringComparison.InvariantCultureIgnoreCase)) != null)
                // {
                //     post.categories.Add(catItems.FirstOrDefault(e => string.Equals(e.Name, post.category, StringComparison.InvariantCultureIgnoreCase)).Id);
                // }
                // else
                // {
                //     var newCat = await auth.CreateCat(post.category);
                //     if (!newCat.IsSuccessStatusCode)
                //     {
                //         throw new Exception(nameof(post));
                //     }
                //     var res = JsonConvert.DeserializeObject<Category>(newCat.Content.ToString());
                //     post.categories.Add(res.Id);
                // }
                post.categories.Add(25);
                // post.tags = new List<string>(){};
                var result = await auth.PostAsync(post);
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception(nameof(post));
                }
            }
            else
            {
                throw new Exception("invalid/expired token");
                
            }
            
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

    public class Category
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Parent { get; set; }
        public List<int> Meta { get; set; } = new List<int>();

    }
}
