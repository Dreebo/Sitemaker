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
    }
}