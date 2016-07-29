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
        public string PathLogo { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Page { get; set;}
        public string TemplateId { get; set; }
        public string MenuId { get; set; }
    }
}