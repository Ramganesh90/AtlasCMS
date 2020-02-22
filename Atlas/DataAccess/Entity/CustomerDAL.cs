using Atlas.DAL;
using Atlas.Models;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public static class CustomerDAL
    {
        private static string _myConnection;

        static CustomerDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        public static SAL02_Contacts getEditContact(int id)
        {
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.Text,
                "Select top 1 * from SAL02_Contacts where SalContId = @CustId",
                new SqlParameter("@CustId", id)).Tables[0].AsEnumerable();
            return getCustomerCollection(dataSet)[0];
        }

        public static List<SAL02_Contacts> searchContact(string lastName, string zipCode)
        {
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                "spATL_CRM_Cont_Filter",
                new SqlParameter("@lastname", lastName),
                new SqlParameter("@zipcode", zipCode)).Tables[0].AsEnumerable();
            return getCustomerCollection(dataSet);
        }

        public static int saveCustomer(SAL02_Contacts modelContact)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();
                if (modelContact.SalContId > 0)
                {
                    parametersList.Add(new SqlParameter("@ContId", modelContact.SalContId));
                    spName = "spATL_CRM_Cont_Upd";

                }
                else
                {
                    spName = "spATL_CRM_Cont_Ins";
                }

                parametersList.Add(new SqlParameter("@CompId", modelContact.SalCompId));
                parametersList.Add(new SqlParameter("@FName", modelContact.SalContFirstName));
                parametersList.Add(new SqlParameter("@LName", modelContact.SalContLastName));
                parametersList.Add(new SqlParameter("@Address", modelContact.SalContAddress));
                parametersList.Add(new SqlParameter("@City", modelContact.SalContCity));
                parametersList.Add(new SqlParameter("@State", modelContact.SalContState));
                parametersList.Add(new SqlParameter("@ZipCode", modelContact.SalContZip));
                parametersList.Add(new SqlParameter("@Phone", modelContact.SalContPhone));
                parametersList.Add(new SqlParameter("@Ext", modelContact.SalContPhoneExt));
                parametersList.Add(new SqlParameter("@Fax", modelContact.SalContFax));
                parametersList.Add(new SqlParameter("@Mobile", modelContact.SalContMobile));
                parametersList.Add(new SqlParameter("@EMail", modelContact.SalContEmail));
                parametersList.Add(new SqlParameter("@ActiveFlag", modelContact.SalContActiveFlag));
                parametersList.Add(new SqlParameter("@LedSource", modelContact.LedSourceId));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);

                result = Convert.ToInt32(resultSet.Rows[0]["ID"]);
            }
            catch (Exception ex)
            {

            }
            return (result);
        }

        public static CustomerProfileViewModel getCustomerProfile(int id)
        {
            DataSet dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                "spATL_CRM_Cont_Profile",  new SqlParameter("@ContId", id));

            CustomerProfileViewModel profile = new CustomerProfileViewModel();
            if (dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var contact = dataSet.Tables[0].Rows;
                    profile.sal02_Contact = new SAL02_Contacts();
                    profile.sal02_Contact.SalContId = Convert.ToInt32(contact[0]["SalContId"]);
                    profile.sal02_Contact.SalContFirstName = Common.ToTitleCase(contact[0]["SalContFirstName"].FormatTrim());
                    profile.sal02_Contact.SalContLastName = Common.ToTitleCase(contact[0]["SalContLastName"].FormatTrim());
                    profile.sal02_Contact.SalContAddress = contact[0]["SalContAddress"].FormatTrim();
                    profile.sal02_Contact.SalContCity = contact[0]["SalContCity"].FormatTrim();
                    profile.sal02_Contact.SalContState = contact[0]["SalContState"].FormatTrim();
                    profile.sal02_Contact.SalContZip = contact[0]["SalContZip"].FormatTrim();
                    profile.sal02_Contact.SalContPhone = contact[0]["SalContPhone"].FormatTrim();
                    profile.sal02_Contact.SalContPhoneExt = contact[0]["SalContPhoneExt"].FormatTrim();
                    profile.sal02_Contact.SalContFax = contact[0]["SalContFax"].FormatTrim();
                    profile.sal02_Contact.SalContMobile = contact[0]["SalContMobile"].FormatTrim();
                    profile.sal02_Contact.SalContEmail = contact[0]["SalContEmail"].FormatTrim();
                    profile.sal02_Contact.SalContActiveFlag = contact[0]["SalContActiveFlag"].FormatTrim() == "Y" ? "Yes" : "No";
                    profile.sal02_Contact.SAL01_Company = new SAL01_Company();
                    profile.sal02_Contact.SAL01_Company.SalCompName = contact[0]["SalCompName"].FormatTrim();
                    profile.sal02_Contact.PRJ06_LedSource = new PRJ06_LedSource();
                    profile.sal02_Contact.PRJ06_LedSource.LedSourceName = contact[0]["LedSourceName"].FormatTrim();
                    if (contact[0]["SalContDateCreated"] != DBNull.Value)
                    {
                        profile.sal02_Contact.SalContDateCreated = Convert.ToDateTime(contact[0]["SalContDateCreated"]);
                    }
                }
                profile.sal03_ResAppointments = new List<SAL03_ResAppointments>();
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    var schedule = dataSet.Tables[1].Rows;
                    for (int i = 0; i < schedule.Count; i++)
                    {
                        SAL03_ResAppointments objResAppointments = new SAL03_ResAppointments();
                        if (schedule[i]["SalApptStartDate"] != DBNull.Value)
                        {
                            objResAppointments.SalApptStartDate = schedule[i]["SalApptStartDate"].FormatTrim();
                        }
                        if (schedule[i]["SalApptStartTime"] != DBNull.Value)
                        {
                            objResAppointments.SalApptStartTime = AppointmentsDAL.getComboLookupValue("Setup40_Time", "Setup40TimeID", "Setup40Time",(schedule[i]["SalApptStartTime"]).FormatTrim());
                        }
                        profile.sal03_ResAppointments.Add(objResAppointments);
                    }
                }

                profile.prj01_Headers = new List<PRJ01_Headers>();
                if (dataSet.Tables[2].Rows.Count > 0)
                {
                    var projects = dataSet.Tables[2].Rows;
                    for (int i = 0; i < projects.Count; i++)
                    {
                        PRJ01_Headers objProjetcs = new PRJ01_Headers();
                        objProjetcs.PRJID = Convert.ToInt32(projects[i]["PRJID"]);
                        if (projects[i]["PRJDateActivated"] != DBNull.Value)
                        {
                            objProjetcs.PRJDateActivated = Convert.ToDateTime(projects[i]["PRJDateActivated"]);
                        }
                        objProjetcs.CommID = projects[i]["CommID"].FormatTrim();
                        objProjetcs.JobNumber = projects[i]["JobNumber"].FormatTrim();
                        objProjetcs.ProjectName = projects[i]["ProjectName"].FormatTrim();
                        objProjetcs.JobStatusId = projects[i]["JobStatusId"].FormatTrim();
                        objProjetcs.PRJNotes = projects[i]["PRJNotes"].FormatTrim();
                        profile.prj01_Headers.Add(objProjetcs);
                    }
                }
                profile.projectNotes = new List<Notes>();
                if (dataSet.Tables[3].Rows.Count > 0)
                {
                    var notes = dataSet.Tables[3].Rows;
                    for (int i = 0; i < notes.Count; i++)
                    {
                        Notes objNotes = new Notes();
                        if (notes[i]["PRJDateEntered"] != DBNull.Value)
                        {
                            objNotes.PRJDateEntered = Convert.ToDateTime(notes[i]["PRJDateEntered"]);
                        }
                        objNotes.PRJNotes = notes[i]["PRJNotes"].FormatTrim();
                        profile.projectNotes.Add(objNotes);
                    }
                }
            }

            return profile;
        }

        public static bool deleteContact(int id)
        {
            int result = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure,
                "spATL_CRM_Cont_Del",
                new SqlParameter("@ConId", id));
            return result > 0;
        }

        public static List<SAL02_Contacts> getCustomerCollection(dynamic dataSet)
        {
            List<SAL02_Contacts> lstContacts = new List<SAL02_Contacts>();

            foreach (var record in dataSet)
            {
                SAL02_Contacts contacts = new SAL02_Contacts();
                if (!record["SalContId"].Equals(DBNull.Value))
                {
                    contacts.SalContId = Convert.ToInt32(record["SalContId"]);
                }
                if (!record["SalContFirstName"].Equals(DBNull.Value))
                {
                    contacts.SalContFirstName = Common.ToTitleCase(Convert.ToString(record["SalContFirstName"]));
                }
                if (!record["SalContLastName"].Equals(DBNull.Value))
                {
                    contacts.SalContLastName = Common.ToTitleCase(Convert.ToString(record["SalContLastName"]));
                }
                if (!record["SalContAddress"].Equals(DBNull.Value))
                {
                    contacts.SalContAddress = Convert.ToString(record["SalContAddress"]);
                }
                if (!record["SalContCity"].Equals(DBNull.Value))
                {
                    contacts.SalContCity = Convert.ToString(record["SalContCity"]);
                }
                if (!record["SalContState"].Equals(DBNull.Value))
                {
                    contacts.SalContState = Convert.ToString(record["SalContState"]);
                }
                if (!record["SalContZip"].Equals(DBNull.Value))
                {
                    contacts.SalContZip = Convert.ToString(record["SalContZip"]);
                }
                if (record.Table.Columns.Contains("SalContPhone") && !record["SalContPhone"].Equals(DBNull.Value))
                {
                    contacts.SalContPhone = Convert.ToString(record["SalContPhone"]);
                }
                if (record.Table.Columns.Contains("SalContPhoneExt") && !record["SalContPhoneExt"].Equals(DBNull.Value))
                {
                    contacts.SalContPhoneExt = Convert.ToString(record["SalContPhoneExt"]);
                }
                if (record.Table.Columns.Contains("SalContFax") && !record["SalContFax"].Equals(DBNull.Value))
                {
                    contacts.SalContFax = Convert.ToString(record["SalContFax"]);
                }
                if (record.Table.Columns.Contains("SalContMobile") && !record["SalContMobile"].Equals(DBNull.Value))
                {
                    contacts.SalContMobile = Convert.ToString(record["SalContMobile"]);
                }
                if (record.Table.Columns.Contains("SalContEmail") && !record["SalContEmail"].Equals(DBNull.Value))
                {
                    contacts.SalContEmail = Convert.ToString(record["SalContEmail"]);
                }
                if (record.Table.Columns.Contains("SalCompId") && !record["SalCompId"].Equals(DBNull.Value))
                {
                    contacts.SalCompId = Convert.ToString(record["SalCompId"]);
                }
                if (record.Table.Columns.Contains("SalContActiveFlag") && !record["SalContActiveFlag"].Equals(DBNull.Value))
                {
                    contacts.SalContActiveFlag = Convert.ToString(record["SalContActiveFlag"]);
                }
                if (record.Table.Columns.Contains("LedSourceId") && !record["LedSourceId"].Equals(DBNull.Value))
                {
                    contacts.LedSourceId = Convert.ToInt32(record["LedSourceId"]);
                }
                lstContacts.Add(contacts);
            }

            return lstContacts;
        }

        public static string FormatTrim(this object obj, bool isDate = false)
        {
            string str = string.Empty;
            if (!obj.Equals(DBNull.Value))
            {
                str = Convert.ToString(obj);
            }
            else str = null;
            return str;
        }
    }
}