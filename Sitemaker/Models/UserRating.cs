using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class UserRating
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Star { get; set; }
        public virtual ICollection<Site> Sites { get; set; }
        public virtual ICollection<Medal> Medals { get; set; }

        public UserRating()
        {
            Medals = new List<Medal>();
            Sites = new List<Site>();
        }
    }
}