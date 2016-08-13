using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitemaker.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public bool IsTopBarExicist { get; set; }
        public bool IsSideBarExicist { get; set; }
        public ICollection<MenuItem> TopBar { get; set; }
        public ICollection<MenuItem> SideBar { get; set; }


        public Menu()
        {
            TopBar = new List<MenuItem>();
            SideBar = new List<MenuItem>();

        }


    }
}