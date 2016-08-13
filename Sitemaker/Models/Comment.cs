using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserComment { get; set; }
        public Site Site { get; set; }
        public DateTime Date { get; set; }
    }
}