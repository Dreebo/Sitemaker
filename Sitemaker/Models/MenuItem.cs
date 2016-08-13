using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class MenuItem
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}