// using System.Collections.Generic;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using Crawler.DataContext;
// using Crawler.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Newtonsoft.Json;

// namespace Crawler.Controllers
// {
//     [Route("api/[controller]")]
//     public class PostsController : ControllerBase
//     {
//         private readonly IConfiguration _config;
//         private readonly CrawlerContext _context;
//         public PostsController(IConfiguration config, CrawlerContext context)
//         {
//             _config = config;
//             _context = context;
//         }

//         [HttpGet]
//         public async Task Get()
//         {
//             var post = new PostCreateModel();
//             wp.CreateAsync(post);
//             return;
//         }
//         // [HttpPost]
//         // public async Task<ActionResult<Post>> Post(PostCreateModel post)
//         // {
//         //     var n = await _context.News.FindAsync(35);
//         //     if (n == null)
//         //     {
//         //         return NotFound();
//         //     }
//         //     post = PostCreateModel.Create(n);
//         //     if (post == null)
//         //     {
//         //         return BadRequest();
//         //     }
//         //     HttpClient client = new HttpClient();
//         //     var json = JsonConvert.SerializeObject(post);
//         //     HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
//         //     var result = await client.PostAsync(_config["wpuri"], content);
//         //     var obj = await result.Content.ReadAsStringAsync();
//         //     return Ok();
//         // }
//     }
// }