using Sitemaker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sitemaker
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("DefaultConnection")
        { }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Comment> Comments {get; set;}
        public DbSet<UserRating> Ratings { get; set; }
        public DbSet<Medal> Medals { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>().HasMany(c => c.Ratings)
                .WithMany(s => s.Sites)
                .Map(t => t.MapLeftKey("SiteId")
                .MapRightKey("RatingId")
                .ToTable("SiteUserRating"));
        }
    }
}