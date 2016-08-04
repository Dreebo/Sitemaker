using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string NamePage { get; set; }
        public string HtmlCode { get; set; }
        public string SiteId { get; set; }
        public Site Site { get; set; }
    }
}