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
using Crawler.DataContext;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Crawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        public ActionResult Get()
        {
            var hcm = new Hcmc();
            var result = hcm.GetNews();
            return Ok(result.Result);
        }
    }
}
