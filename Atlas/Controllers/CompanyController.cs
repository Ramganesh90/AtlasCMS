using Atlas.DAL;
using Atlas.DataAccess;
using Atlas.DataAccess.Entity;
using Atlas.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        public ActionResult Index()
        {

            return View();
        }


        public ActionResult Create()
        {
            LoadCompanyDetails();
            return View();
        }

        private void LoadCompanyDetails()
        {
            ViewBag.Title = BusinessConstants.titleNewCompany;
            var lstState = DataAccess.Entity.Common.getStates();
            SelectList statelist = new SelectList(lstState, "stateid", "statename");
            ViewBag.StateList = statelist;
        }

        [HttpPost]
        public ActionResult Create(SAL01_Company modelCompany, string submitCompany)
        {
            try
            {
                if (!modelCompany.EmailExists)
                {
                    modelCompany.SalCompEMail = BusinessConstants.NA;
                    ModelState.Remove("SalCompEMail");
                }
                if (ModelState.IsValid)
                {
                    modelCompany.SalCompPhone = DataAccess.Entity.Common.FormatPhoneText(modelCompany.SalCompPhone);
                    modelCompany.SalCompMobile = DataAccess.Entity.Common.FormatPhoneText(modelCompany.SalCompMobile);
                    modelCompany.SalCompFax = DataAccess.Entity.Common.FormatPhoneText(modelCompany.SalCompFax);

                    DataTable data = CompanyDAL.SaveNewCompany(modelCompany);
                    if (data.Rows.Count > 0)
                    {
                        ViewBag.SavedCompID = data;
                        return RedirectToAction("create", "customers", new { @compId = data.Rows[0]["CompanyId"] });
                    }
                    else
                    {
                        LoadCompanyDetails();
                        ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                        return View(modelCompany);
                    }

                }
                else
                {
                    LoadCompanyDetails();
                    ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                    return View(modelCompany);
                }
            }
            catch (Exception ex)
            {
                LoadCompanyDetails();
                ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                return View(modelCompany);

            }
        }

        [HttpPost]
        public ActionResult getStateAndCity(string zip_prefix)
        {
            var result = (from zip in DataAccess.Entity.Common.getStateAndCityZip(zip_prefix)
                          select new
                          {
                              label = zip.Zipcode,
                              val = zip.Zipcode,
                              state = zip.State,
                              city = zip.Town
                          }).ToList();

            return Json(result);

        }

    }
}
