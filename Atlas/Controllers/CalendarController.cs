using Atlas.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/
        public ActionResult Index()
        {
            ViewBag.Title = "Calendar";
            //this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.AssignToList = new SelectList(AppointmentsDAL.getAssignToList(), "CommID", "FullName");
            return View();
        }
        //
        // GET: /Calendar/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Calendar/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Calendar/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Calendar/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Calendar/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Calendar/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Calendar/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetCalendarEvents(string CommId)
        {
            var calendar = new List<Calendar_Evts>();
            if (!string.IsNullOrWhiteSpace(CommId))
            {
                calendar = AppointmentsDAL.getCalendarEvents(CommId);
            }
            return Json(calendar, JsonRequestBehavior.AllowGet);
        }  
    }
}
