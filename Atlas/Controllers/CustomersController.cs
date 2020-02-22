using Atlas.DAL;
using Atlas.DataAccess;
using Atlas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atlas.DataAccess.Entity;

namespace Atlas.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            ViewBag.Title = BusinessConstants.titleCustomers;
            return View();
        }
        [HttpPost]
        public ActionResult Search(string lastName, string zipCode, string ContId = "")
        {
            if (string.IsNullOrEmpty(ContId))
            {
                var contact = CustomerDAL.searchContact(lastName, zipCode);
                return Json(contact);
            }
            else
            {
                var contact = CustomerDAL.getEditContact(Convert.ToInt32(ContId));
                return Json(contact);
            }
        }

        public ActionResult Create(string id = "")
        {
            LoadCombos();
            id = string.IsNullOrWhiteSpace(id) ? "0" : Convert.ToString(id);
            int ContID = int.Parse(id);

            if (ContID == 0)
            {
                ViewBag.Title = BusinessConstants.titleNewCustomer;
                return View();
            }
            else
            {
                var Contact = CustomerDAL.getEditContact(ContID);
                Contact.EmailExists = true;
                ViewBag.Title = BusinessConstants.titleEditCustomer;
                if (string.IsNullOrWhiteSpace(Contact.SalContEmail))
                {
                    Contact.SalContEmail = BusinessConstants.NA;
                }
                if (Contact.SalContEmail.Equals(BusinessConstants.NA))
                {
                    Contact.SalContEmail = string.Empty;
                    Contact.EmailExists = false;
                }

                return View(Contact);
            }
        }

        private void LoadCombos()
        {
            ViewBag.sal01_Company = BindCompanies();
            var lstSources = CompanyDAL.getLedSources();
            SelectList sourcelist = new SelectList(lstSources, "LedSourceId", "LedSourceName");
            ViewBag.LedSources = sourcelist;
            var lstState = DataAccess.Entity.Common.getStates();
            ViewBag.defaultCompany = Request.QueryString["compId"];
            SelectList statelist = new SelectList(lstState, "stateid", "statename");
            ViewBag.StateList = statelist;
        }

        [HttpPost]
        public ActionResult Create(SAL02_Contacts modelContact, string submitCustomertoAppointment, string submitCustomer,string submitCustomerToProject)
        {
            try
            {
                ModelState.Remove("SalContId");
                if (!modelContact.EmailExists)
                {
                    modelContact.SalContEmail = BusinessConstants.NA;
                    ModelState.Remove("SalContEmail");
                }
                if (ModelState.IsValid)
                {
                    modelContact.SalContPhone = DataAccess.Entity.Common.FormatPhoneText(modelContact.SalContPhone);
                    modelContact.SalContMobile = DataAccess.Entity.Common.FormatPhoneText(modelContact.SalContMobile);
                    modelContact.SalContFax = DataAccess.Entity.Common.FormatPhoneText(modelContact.SalContFax);
                    var result = CustomerDAL.saveCustomer(modelContact);
                    if (result > 0)
                    {
                        if (submitCustomertoAppointment != null)
                        {
                            return RedirectToAction("create", "appointments", new { id = result, assign = "1" });
                        }
                        else if (submitCustomerToProject != null && submitCustomerToProject == "save")
                        {
                            return RedirectToAction("index", "project");
                        }
                        else if (submitCustomer != null)
                        {
                            return RedirectToAction("index", "customers");
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                        LoadCombos();
                        ViewBag.Title = string.IsNullOrWhiteSpace(Convert.ToString(modelContact.SalContId)) ?
                                            BusinessConstants.titleNewCustomer : BusinessConstants.titleEditCustomer;
                        return View(modelContact);
                    }

                }
               
                    LoadCombos();
                    ViewBag.Title = string.IsNullOrWhiteSpace(Convert.ToString(modelContact.SalContId)) ?
                                        BusinessConstants.titleNewCustomer : BusinessConstants.titleEditCustomer;
                    return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message + BusinessConstants.contactAdmin);
                LoadCombos();
                ViewBag.Title = string.IsNullOrWhiteSpace(Convert.ToString(modelContact.SalContId)) ?
                                            BusinessConstants.titleNewCustomer : BusinessConstants.titleEditCustomer;
                return View();
            }
        }

        public ActionResult Details(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ViewBag.Title = BusinessConstants.titleCustomerProfile;
                    var profile = CustomerDAL.getCustomerProfile(int.Parse(id));
                    profile.sal02_Contact.SalContPhone = DataAccess.Entity.Common.FormatPhoneText(profile.sal02_Contact.SalContPhone);
                    profile.sal02_Contact.SalContMobile = DataAccess.Entity.Common.FormatPhoneText(profile.sal02_Contact.SalContMobile);
                    profile.sal02_Contact.SalContFax = DataAccess.Entity.Common.FormatPhoneText(profile.sal02_Contact.SalContFax);
                    return View(profile);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                return RedirectToAction("index");
            }
        }

        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                CustomerDAL.deleteContact(int.Parse(id));
                return RedirectToAction("index");
            }
            else
                return View();
        }

        private static List<SelectListItem> BindCompanies()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            var companies = CompanyDAL.getAllCompanies();
            foreach (var item in companies)
            {
                items.Add(new SelectListItem
                {
                    Text = item.SalCompName,
                    Value = item.SalCompId
                });
            }
            return items;
        }
    }
}
