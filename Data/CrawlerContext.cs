using Crawler.Models;
using Microsoft.EntityFrameworkCore;

namespace Crawler.DataContext
{
    public class CrawlerContext : DbContext
    {
        public DbSet<New> News { get; set; }
        public DbSet<Website> Sites { get; set; }
        public CrawlerContext(DbContextOptions<CrawlerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vne>().HasBaseType<Website>();
            modelBuilder.Entity<Vne>().HasDiscriminator<string>("Discriminator").HasValue("VNE");
        }
    }
}