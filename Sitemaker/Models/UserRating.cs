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

        public virtual ICollection<Site> Sites { get; set; }

        public UserRating()
        {
            Sites = new List<Site>();
        }
    }
}