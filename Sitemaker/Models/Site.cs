using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public ICollection<Page> Pages { get; set;}
        public int TemplateId { get; set; }
        public int MenuId { get; set; }

        public Site()
        {
            Pages = new List<Page>();
        }
    }
}