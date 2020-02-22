using Atlas.DAL;
using Atlas.DataAccess;
using Atlas.DataAccess.Entity;
using Atlas.Models;
using Atlas.Models.DBO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //public Actionre AdminController()
        // {
        //     if (string.IsNullOrWhiteSpace(Convert.ToString(Session["LoggedInUser"])) &&
        //        string.IsNullOrWhiteSpace(Convert.ToString(Session["Role"])) &&
        //        (Convert.ToString(Session["Role"]) == "A") ||
        //        (Convert.ToString(Session["Role"]) == "M"))
        //     {
        //         return RedirectToAction("Login", "Account");
        //     }
        // }
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["LoggedInUser"] != null)
            {
                Session["PRJID"] = null;
                List<PRL01_Employees> Salesman = AdminDal.getSalesman();
                var salesEmployeesList = Salesman.Select(i => new
                {
                    EmployeeID = i.EmployeeID,
                    Salesman = i.LastName + " " + i.FirstName
                });
                ViewBag.salesEmployeesList = new SelectList(salesEmployeesList, "EmployeeID", "Salesman");
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult Search()
        {

            return View();
        }
        [ActionName("billing")]
        public ActionResult SearchBilling()
        {

            return View("Billing/Search");
        }


        [HttpPost]
        public JsonResult searchList(string projectName, string City)
        {
            List<SearchPunchListModel> PunchList= new List<SearchPunchListModel>();
            if (!string.IsNullOrWhiteSpace(projectName) && !string.IsNullOrWhiteSpace(City))
            {
                PunchList = AdminDal.searchPunchList(projectName, City);
            }
            return Json(PunchList);
        }

        [HttpPost]
        public JsonResult UpdateProjectBid(int id, string action, string title)
        {
            int result = -2;
            if (title == "job")
            {
                result = AdminDal.UpdateProject(id, action);
            }
            else if (title == "bid")
            {
                result = AdminDal.UpdateBid(id, action);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult UpdateCommission(string ID)
        {
            if (Session["Role"] != null && (Convert.ToString(Session["Role"]) == "A" || Convert.ToString(Session["Role"]) == "M"))
            {
                Session["CommId"] = ID;
                return Json(1);
            }
            return Json(-2);
        }

        public ActionResult ComplaintForm(string id)
        {
            var resultData = AdminDal.getComplaintData(Convert.ToInt32(id));
            return View(resultData);
        }
        [HttpPost]
        public ActionResult saveComplaint(Complaints modelComplaint)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["LoggedInUser"])))
                {
                    if (String.IsNullOrWhiteSpace(modelComplaint.JobNotes))
                    {
                        return View(modelComplaint);
                    }

                    var ComplaintId = AdminDal.saveComplaint(modelComplaint, Session["LoggedInUser"].ToString());
                    if (ComplaintId > 0)
                    {
                        string Subject = modelComplaint.JobNumber + " " + modelComplaint.JobName + " -- PUNCHLIST ITEM ID " + ComplaintId;
                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/content/template/PunchListTemplate.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{punchlistid}", ComplaintId.ToString());
                        body = body.Replace("{jobnumber}", modelComplaint.JobNumber);
                        body = body.Replace("{jobname}", modelComplaint.JobName);
                        body = body.Replace("{jobphone}", modelComplaint.JobPhone);
                        body = body.Replace("{contact}", modelComplaint.JobContact);
                        body = body.Replace("{salesman}", modelComplaint.Salesman);
                        body = body.Replace("{complaint}", modelComplaint.JobNotes);

                        MailUtil mailUtil = new MailUtil(Subject, body.ToString());
                        mailUtil.Send();
                        return RedirectToAction("search","admin");
                    }
                }
                else
                {
                   return RedirectToAction("Login", "Account");
                }


            }
            catch (Exception e)
            {
                Logger.SaveErr(e);
                ModelState.AddModelError("Error", BusinessConstants.ValidateEntries);
                return View();
            }
            return RedirectToAction("search", "admin");
        }

        [HttpPost]
        public JsonResult viewPunchList()
        {
            var role = Convert.ToString(Session["Role"]);
            var user = Convert.ToString(Session["UserId"]);
            return Json(AdminDal.ViewPunchlist(role,user));
        }

        [HttpPost]
        public JsonResult UpdatePunchListStatus(int punchId, string status)
        {
            int result = -2;
            if (punchId > 0)
            {
                result = AdminDal.UpdatePunchListStatus(punchId, status);
            }
            return Json(result);
        }

        public ActionResult maintenance(string id)
        {
            return View("billing/projmaintenance");
        }
        public ActionResult aiabilling(string id)
        {
            ViewBag.UOMTypes = new SelectList(Atlas.DataAccess.Entity.Common.getUnitofMeasure(), "UnitOfMeasure", "UomDescription");
           //ar BidItems = ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)).Select(i=>i.BidItemId);
            ViewBag.BidItems = new SelectList(ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)),"BidItemId", "BidItemId");
            //ViewBag.BidItems = new SelectList(BidItems, "BidItemId", "BidItemId");
            return View("billing/aiabilling");
        }

        [HttpPost]
        public ActionResult saveMaintenance(Contract modelContract)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(modelContract.PRJID)) && Convert.ToInt32(modelContract.PRJID)>0)
                {
                    var ResultId = AdminDal.saveMaintenance(modelContract);
                    if (ResultId > 0)
                    {
                        return RedirectToAction("billing", "admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", BusinessConstants.ValidateEntries);
                        return View("billing/projmaintenance", modelContract);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Project cannot be saved! Please try again");
                    return View("billing/projmaintenance", modelContract);
                }
            }
            catch (Exception e)
            {
                Logger.SaveErr(e);
                ModelState.AddModelError("Error", BusinessConstants.contactAdmin);
                return View("billing/projmaintenance", modelContract);
            }
        }

        [HttpPost]
        public ActionResult saveAiabilling(AIABilling modelBilling)
        {
            int id = modelBilling.PRJID;
            try
            {
                modelBilling.NetChangeByCO = Convert.ToDecimal(modelBilling.NetChangeByCO);
                modelBilling.OriginalContractSum = Convert.ToDecimal(modelBilling.OriginalContractSum);
                modelBilling.UnitPrice = Convert.ToDecimal(modelBilling.UnitPrice).ToString();
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(modelBilling.PRJID)) && Convert.ToInt32(modelBilling.PRJID) > 0)
                    {
                        var ResultId = AdminDal.saveAiaBilling(modelBilling);
                        if (ResultId > 0)
                        {
                            return RedirectToAction("billing", "admin");
                        }
                        else
                        {
                            ViewBag.UOMTypes = new SelectList(Atlas.DataAccess.Entity.Common.getUnitofMeasure(), "UnitOfMeasure", "UomDescription");
                            //ar BidItems = ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)).Select(i=>i.BidItemId);
                            ViewBag.BidItems = new SelectList(ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)), "BidItemId", "BidItemId");
                            ModelState.AddModelError("", BusinessConstants.ValidateEntries);
                            return View("billing/aiabilling", modelBilling);
                        }
                    }
                    else
                    {
                        ViewBag.UOMTypes = new SelectList(Atlas.DataAccess.Entity.Common.getUnitofMeasure(), "UnitOfMeasure", "UomDescription");
                        //ar BidItems = ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)).Select(i=>i.BidItemId);
                        ViewBag.BidItems = new SelectList(ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)), "BidItemId", "BidItemId");
                        ModelState.AddModelError("", "Project cannot be saved! Please try again");
                        return View("billing/aiabilling", modelBilling);
                    }
                }
                else
                {
                    ViewBag.UOMTypes = new SelectList(Atlas.DataAccess.Entity.Common.getUnitofMeasure(), "UnitOfMeasure", "UomDescription");
                    //ar BidItems = ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)).Select(i=>i.BidItemId);
                    ViewBag.BidItems = new SelectList(ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)), "BidItemId", "BidItemId");
                    ModelState.AddModelError("", BusinessConstants.ValidateEntries);
                    return View("billing/aiabilling", modelBilling);
                }
            }
            catch (Exception e)
            {
                ViewBag.UOMTypes = new SelectList(Atlas.DataAccess.Entity.Common.getUnitofMeasure(), "UnitOfMeasure", "UomDescription");
                //ar BidItems = ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)).Select(i=>i.BidItemId);
                ViewBag.BidItems = new SelectList(ProjectDAL.GetBidItemsByProject(Convert.ToInt32(id)), "BidItemId", "BidItemId");
                Logger.SaveErr(e);
                ModelState.AddModelError("", BusinessConstants.contactAdmin);
                return View("billing/aiabilling",modelBilling);
            }
        }

        [HttpPost]
        public JsonResult getAiaBilling(string PRJID,string BIDID)
        {
            AIABilling modelBilling = null;
            if (!string.IsNullOrWhiteSpace(PRJID) && !string.IsNullOrWhiteSpace(BIDID))
            {
              modelBilling = AdminDal.getAIABilling(PRJID, BIDID);
            }
            return Json(modelBilling);
        }
    }
}