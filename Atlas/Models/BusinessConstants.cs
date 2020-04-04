using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Models
{
    public class BusinessConstants
    {
        public const string LoginFailed = "Invalid Login Attempts";
        public const string DuplicateLogin = "Username already registered. Please login!";
        public const string UserNotFound = "No matching Username/Email Found";
        public const string PasswordUpdated = "Your profile password has been updated. Please Login!";

        public const string titleCustomers = "Customers";
        public const string titleNewCustomer = "Enter New Customer";
        public const string titleEditCustomer = "Edit Customer";
        public const string titleCustomerProfile = "Customer Account";
        public const string titleNewCompany = "Enter New Company";
        public const string titleEditCompany = "Edit Company";

        public const string DETAILSNOTFOUND = "No Details Available";

        public const string QuotedProjectsNA = "No Quoted Projects available";
        public const string ActiveProjectsNA = "No Active/Pending Projects available";
        public const string CompletedProjectsNA = "No Completed Projects available";
        public const string ProjectNotesNA = "No Projects Notes available";
        public const string NoAppointments = "No Appointments";

        #region Appointments
        public const string titleAppointments = "Appointment";
        public const string titleNewAppointments = "Add Appointment";
        public const string titleEditAppointments = "Edit Appointment";
        #endregion

        #region Personal Appointments
        public const string titleAddPersonalAppointments = "Add Personal Appointment";
        public const string titleEditPersonalAppointments = "Edit Personal Appointment";
        #endregion

        #region Common
        public const string ValidateEntries = "Validation Failed. Please check the entries";
        public const string NA = "N/A";
        public const string duplicateRecord = "Record already exists. Please check the entry.";
        public const string contactAdmin = "Error occurred. Please contact Administrator or try again!";
        #endregion

        public const string GoogleMap = "https://www.google.com/maps/dir";
        public const string GoogleMapEmbed = "https://www.google.com/maps/embed?";
        public const string AtlasAddress = "30 Northeast Industrial Road Branford, CT 06405";
        public const int DefaultBillingId = 1;
    }
}