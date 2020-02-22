using Atlas.DataAccess.Entity;
using Atlas.Models.DBO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public class QuoteController : Controller
    {

        // GET: /Quote/
        [Route("quote/id/{prjid}")]
        public ActionResult Index(string prjid)
        {
            //string Title = this.ControllerContext.RouteData.Values["controller"].ToString();
            if (!string.IsNullOrWhiteSpace(prjid))
            {
                Session["PRJID"] = prjid;
                ViewBag.PRJID = prjid;

                var model = QuoteDal.getQuotesForDisplay(prjid);
                ViewBag.Title = string.Format("{0} #{1} - {2}", "Project", prjid, model.FirstOrDefault()?.ProjectName);
                return View(model);
            }
            else
            {
                return RedirectToRoute("Home");
            }
        }

        //
        // GET: /Quote/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Quote/Create
        [Route("quote/project/{PRJID}/create")]
        [Route("quote/project/{PRJID}/group/{group}/edit")]
        public ActionResult Create(string PRJID = "", string group = "")
        {
            if (!string.IsNullOrWhiteSpace(PRJID))
            {
                Session["PRJID"] = PRJID;
            }
            if (!string.IsNullOrWhiteSpace((string)Session["CommID"]) && Session["PRJID"] != null && Convert.ToInt32(Session["PRJID"]) > 0)
            {
                ViewBag.Title = string.Format("{0}", Session["PRJID"]);
                var id = Convert.ToInt32(Session["PRJID"]);
                var model = new List<BidItems>();
                if (id > 0)
                {
                    model = ProjectDAL.GetBidItemsByProject(id);
                }
                if (!string.IsNullOrWhiteSpace(group))
                {
                    int Qgroup = 0;
                    Qgroup = Int32.Parse(group);
                    var selectedQuote = QuoteDal.getQuotesForDisplay(PRJID).Where(i => i.QuoteGroup == Convert.ToInt32(Qgroup)).Select(i => i.QuotesItem);
                    var selectedBids = selectedQuote.FirstOrDefault().Select(i => i.BIDID).ToList();
                    Session["QGroup"] = Qgroup;
                    foreach (var item in selectedBids)
                    {
                        if (model.Find(i => i.BidItemId.Equals(item.ToString())) != null)
                        {
                            model.Find(i => i.BidItemId.Equals(item.ToString())).SelectToQuote = true;
                        }
                    }
                }
                else
                {
                    Session["QGroup"] = 0;
                }
                return View(model);
            }
            else
            {
                return Redirect("index");
            }
        }

        [HttpPost]
        public JsonResult CreateQuote(string PRJID, string BIDID)
        {
            int result = 0;
            try
            {
                int group = Session["QGroup"] != null ? (int)Session["QGroup"] : 0;
                result = QuoteDal.AddQuotesForProject(PRJID, BIDID, group);

            }
            catch (Exception ex)
            {
                result = -2;
                Logger.SaveErr(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(string PRJID, string BidId, string QuoteGroup)
        {
            int result = -2;
            try
            {
                result = QuoteDal.DeleteQuotes(PRJID, BidId, QuoteGroup);

            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -2;
            }
            return Json(result);
        }

    }
}
