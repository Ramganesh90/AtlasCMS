using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string MenuLink { get; set; }
    }
}