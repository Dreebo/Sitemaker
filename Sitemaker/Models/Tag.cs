using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [StringLength(20)]
        [Index("NameIndex", IsUnique = true)]
        public string Name { get; set; }
        public ICollection<Site> Sites { get; set; }

        public Tag()
        {
            Sites = new List<Site>();
        }
    }
}