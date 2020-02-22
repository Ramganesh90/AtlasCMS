using Atlas.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Atlas.Controllers
{
    public class PartialController : Controller
    {
        //
        // GET: /Partial/   

        public ActionResult Index()
        {
            return View();
        }


        [ChildActionOnly]
        public ActionResult _MenuPartial()
        {
            List<Menu> lstMenu = GetMenuList();
            return PartialView(lstMenu);
        }

        [ChildActionOnly]
        public ActionResult _ProjectMenu()
        {
            List<Menu> lstMenu = GetMenuList();
            return PartialView("_ProjectMenu",lstMenu);
        }

        private List<Menu> GetMenuList()
        {
            string menuListPath = Server.MapPath(ConfigurationManager.AppSettings["menuListPathConfig"].ToString());

            var menuDoc = XDocument.Load(menuListPath);
            var menuList = menuDoc.Descendants("Menu").Select(d =>
                         new
                         {
                             MenuId = d.Element("MenuId").Value,
                             Name = d.Element("Name").Value,
                             MenuLink = d.Element("MenuLink").Value,
                             Level = d.Element("Level").Value
                         }).ToList();

            List<Menu> lstMenu = new List<Menu>();
            foreach (var menuCollection in menuList)
            {
                Menu menu = new Menu
                {
                    Name = menuCollection.Name,
                    Level = Convert.ToInt32(menuCollection.Level),
                    MenuId = Convert.ToInt32(menuCollection.MenuId),
                    MenuLink = menuCollection.MenuLink,
                };
                lstMenu.Add(menu);
            }

            return lstMenu;
        }
    }
}