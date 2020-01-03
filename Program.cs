using Microsoft.AspNetCore.Hosting;
using Crawler.DataContext;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
namespace Crawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
            .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
