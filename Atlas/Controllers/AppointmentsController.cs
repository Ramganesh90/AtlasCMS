using Atlas.DAL;
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
    public class AppointmentsController : Controller
    {
        //
        // GET: /Appointment/

        public ActionResult Index()
        {
            ViewBag.Title = BusinessConstants.titleAppointments+"s";
            return View();
        }
        //
        // GET: /Appointment/Details/5

        public ActionResult Details(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ViewBag.Title = BusinessConstants.titleAppointments;
                    var appointments = AppointmentsDAL.getAppointmentDetails(int.Parse(id));
                    
                    if(appointments.PersonalAppointments != null) {
                        ViewBag.Title = BusinessConstants.titleEditPersonalAppointments;
                        LoadPersonalAppointmentCombos();
                        ViewBag.StartTime = AppointmentsDAL.getComboLookupValue("Setup40_Time", "Setup40TimeID", "Setup40Time", appointments.PersonalAppointments.SalApptStartTime);
                        ViewBag.EndTime = AppointmentsDAL.getComboLookupValue("Setup40_Time", "Setup40TimeID", "Setup40Time", appointments.PersonalAppointments.SalApptEndTime);
                        return View("personal", appointments.PersonalAppointments);
                    }
                    appointments.ScheduledAppointments.CommID = AppointmentsDAL.getCommissionName(appointments.ScheduledAppointments.CommID);
                     appointments.ScheduledAppointments.SalApptUserEntered = AppointmentsDAL.getUsername(appointments.ScheduledAppointments.SalApptUserEntered);
                     appointments.ScheduledAppointments.SalApptPhone = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptPhone);
                     appointments.ScheduledAppointments.SalApptPhoneExt = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptPhoneExt);
                     appointments.ScheduledAppointments.SalApptFax = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptFax);
                    appointments.ScheduledAppointments.SalApptMobile = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptMobile);
                    SetViewBagForCombos(appointments.ScheduledAppointments);
                    return View(appointments.ScheduledAppointments);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }

        private void SetViewBagForCombos(SAL03_ResAppointments appointments)
        {
            ViewBag.StartTime = AppointmentsDAL.getComboLookupValue("Setup40_Time","Setup40TimeID", "Setup40Time", appointments.SalApptStartTime);
            ViewBag.EndTime = AppointmentsDAL.getComboLookupValue("Setup40_Time", "Setup40TimeID", "Setup40Time", appointments.SalApptEndTime);
            ViewBag.FenceType = AppointmentsDAL.getComboLookupValue("Setup25_FenceTypes", "FenceTypeID", "FenceType", Convert.ToString(appointments.FenceTypeID));
            ViewBag.SalesType = AppointmentsDAL.getComboLookupValue("SAL10_SalesTypes", "SalTypeId", "SalType", Convert.ToString(appointments.SalTypeId));
        }

        //
        // GET: /Appointment/Create

        public ActionResult Create(string id = "", string assign="0")
        {
            LoadAppointmentCombos();
            id = string.IsNullOrWhiteSpace(id) ? "0" : Convert.ToString(id);
            int ApptId = int.Parse(id);

           

            if (ApptId == 0 || assign.Equals("1"))
            {
                ViewBag.Title = BusinessConstants.titleNewAppointments;
                var Appointment = new SAL03_ResAppointments();
                Appointment.SalApptStartDate = DateTime.Now.ToShortDateString();
                Appointment.SalApptEndDate = DateTime.Now.ToShortDateString();

                return View(Appointment);
            }
            else
            {
                var Appointment = AppointmentsDAL.getEditAppointment(ApptId);
                ViewBag.Title = BusinessConstants.titleEditAppointments;
                Appointment.SalApptStartDate = Convert.ToDateTime(Appointment.SalApptStartDate).ToShortDateString();
                Appointment.SalApptEndDate = Convert.ToDateTime(Appointment.SalApptStartDate).ToShortDateString();
                if (String.IsNullOrWhiteSpace(Appointment.SalApptEmail) ||
                    Appointment.SalApptEmail.Equals(BusinessConstants.NA))
                {
                    Appointment.SalApptEmail = string.Empty;
                    Appointment.EmailExists = false;
                }
                else
                {
                    Appointment.EmailExists = true;
                }
                return View(Appointment);
            }
        }

        private void LoadAppointmentCombos()
        {
            ViewBag.FenceTypesList = new SelectList(AppointmentsDAL.getFenceTypesList(), "FenceTypeID", "FenceType");
            ViewBag.SalesTypeCallList = new SelectList(AppointmentsDAL.getSalesTypeList(), "SalTypeId", "SalType");
            ViewBag.StatesList = new SelectList(DataAccess.Entity.Common.getStatesList(), "State", "StateName");
            ViewBag.ShowApptTime = new SelectList(AppointmentsDAL.getTime(), "Setup40TimeID", "Setup40Time");
            ViewBag.AssignToList = new SelectList(AppointmentsDAL.getAssignToList(), "CommID", "FullName");

        }

        //
        // POST: /Appointment/Create

        [HttpPost]
        public ActionResult Create(SAL03_ResAppointments modelAppt)
        {
            try
            {
                ModelState.Remove("SalApptId");
                if (!modelAppt.EmailExists)
                {
                    modelAppt.SalApptEmail = BusinessConstants.NA;
                    ModelState.Remove("SalApptEmail");
                }

                if (ModelState.IsValid)
                {
                    modelAppt.SalApptUserEntered = User.Identity.Name;
                    if (AppointmentsDAL.SaveAppointment(modelAppt))
                    {
                        return RedirectToAction("index", "calendar");
                    }
                    else
                    {
                        LoadAppointmentCombos();
                        ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                        return View(modelAppt);
                    }

                }
                else
                {
                    LoadAppointmentCombos();
                    ModelState.AddModelError(String.Empty,BusinessConstants.ValidateEntries);
                    return View(modelAppt);
                }
            }
            catch (Exception ex)
            {
                LoadAppointmentCombos();
                ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                return View(modelAppt);

            }
        }

        public ActionResult savePersonalAppt(Personal_Appointment modelAppt)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    modelAppt.SalApptUserEntered = User.Identity.Name;
                    if (AppointmentsDAL.SaveAppointment(modelAppt))
                    {
                        return RedirectToAction("index", "calendar");
                    }
                    else
                    {
                        LoadPersonalAppointmentCombos();
                        ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                        return View("personal",modelAppt);
                    }

                }
                else
                {
                    LoadPersonalAppointmentCombos();
                    ModelState.AddModelError(String.Empty, BusinessConstants.ValidateEntries);
                    return View("personal", modelAppt);
                }
            }
            catch (Exception ex)
            {
                LoadPersonalAppointmentCombos();
                ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                return View("personal", modelAppt);

            }
        }

        //
        // GET: /Appointment/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

               //
        // GET: /Appointment/Delete/5

        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AppointmentsDAL.deleteAppointment(int.Parse(id));
                return RedirectToAction("index");
            }
            else
                return RedirectToAction("create", "appointments", id);
        }

        //
        // POST: /Appointment/Delete/5

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

        [HttpPost]
        public ActionResult Search(string lastName, string city, string zipCode)
        {
            var appointment = AppointmentsDAL.searchAppointment(lastName, city, zipCode);
            return Json(appointment);
        }
        [HttpPost]
        public ActionResult ShowApptById(string id)
        {
            var appointments = new ATL_Appointments();
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    appointments = AppointmentsDAL.getAppointmentDetails(int.Parse(id));
                    appointments.ScheduledAppointments.CommID = AppointmentsDAL.getCommissionName(appointments.ScheduledAppointments.CommID);
                    appointments.ScheduledAppointments.SalApptUserEntered = AppointmentsDAL.getUsername(appointments.ScheduledAppointments.SalApptUserEntered);
                    appointments.ScheduledAppointments.SalApptPhone = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptPhone);
                    appointments.ScheduledAppointments.SalApptPhoneExt = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptPhoneExt);
                    appointments.ScheduledAppointments.SalApptFax = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptFax);
                    appointments.ScheduledAppointments.SalApptMobile = DataAccess.Entity.Common.FormatPhoneText(appointments.ScheduledAppointments.SalApptMobile);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
            return Json(appointments);
        }

        #region Personal Appointments
        public ActionResult personal()
        {
            LoadPersonalAppointmentCombos();
            ViewBag.Title = BusinessConstants.titleAddPersonalAppointments;
            var Appointment = new Personal_Appointment();
            Appointment.SalApptStartDate = DateTime.Now.ToShortDateString();
            Appointment.SalApptEndDate = DateTime.Now.ToShortDateString();
            return View(Appointment);
        }

        private void LoadPersonalAppointmentCombos()
        {
            ViewBag.ShowApptTime = new SelectList(AppointmentsDAL.getTime(), "Setup40TimeID", "Setup40Time");
            ViewBag.AssignToList = new SelectList(AppointmentsDAL.getAssignToList(), "CommID", "FullName");
            ViewBag.ApptDescriptions = new SelectList(AppointmentsDAL.getAppointmentDescriptions(), "SALApptDescId", "SALApptDesc");
        }


        #endregion

    }
}
