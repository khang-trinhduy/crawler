﻿// <auto-generated />
using System;
using Crawler.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Crawler.Migrations
{
    [DbContext(typeof(CrawlerContext))]
    [Migration("20190619092154_add render")]
    partial class addrender
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Crawler.Models.New", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author");

                    b.Property<string>("Contents");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Publish");

                    b.Property<string>("Rendered");

                    b.Property<string>("Source");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.Property<int?>("WebsiteId");

                    b.HasKey("Id");

                    b.HasIndex("WebsiteId");

                    b.ToTable("News");
                });

            modelBuilder.Entity("Crawler.Models.Website", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Sites");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Website");
                });

            modelBuilder.Entity("Crawler.Models.Vne", b =>
                {
                    b.HasBaseType("Crawler.Models.Website");

                    b.HasDiscriminator().HasValue("VNE");
                });

            modelBuilder.Entity("Crawler.Models.New", b =>
                {
                    b.HasOne("Crawler.Models.Website")
                        .WithMany("News")
                        .HasForeignKey("WebsiteId");
                });
#pragma warning restore 612, 618
        }
    }
}
