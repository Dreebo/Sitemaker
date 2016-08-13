using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public ICollection<Page> Pages { get; set;}
        public int TemplateId { get; set; }
        public bool Pablish { get; set; }
        public DateTime Date { get; set; }
        //public int MenuId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public int RatingCount { get; set; }
        public double AverageRating { get; set; }
        public virtual Menu Menu { get; set; }

        public virtual ICollection<UserRating> Ratings { get; set; }

        public Site()
        {
            Ratings = new List<UserRating>();
            Pages = new List<Page>();
        }
    }
}