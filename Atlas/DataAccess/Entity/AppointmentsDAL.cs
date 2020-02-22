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
    public class AppointmentsDAL
    {
        private static string _myConnection;

        static AppointmentsDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        public static List<Setup25_FenceTypes> getFenceTypesList()
        {
            List<Setup25_FenceTypes> lstFenceType = new List<Setup25_FenceTypes>();

            SqlDataReader fenceResult = null;
            fenceResult = SqlHelper.ExecuteReader(_myConnection, "spddl_FenceType");

            while (fenceResult.Read())
            {
                Setup25_FenceTypes objFenceType = new Setup25_FenceTypes();
                objFenceType.FenceTypeID = Convert.ToInt32(fenceResult["FenceTypeID"]);
                objFenceType.FenceType = Convert.ToString(fenceResult["FenceType"]);

                lstFenceType.Add(objFenceType);
            }
            fenceResult.Close();
            return lstFenceType;
        }

        internal static ATL_Appointments getAppointmentDetails(int apptId)
        {
            var Appointments = new ATL_Appointments();
            var Appointment = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
              "spATL_APPT_DETAILS",
               new SqlParameter("@ApptId", apptId)).Tables[0].AsEnumerable();

            if (Appointment.Count() == 0)
            {
                var PersonalAppt = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                             "spATL_P_APPT_DETAILS",
                 new SqlParameter("@ApptId", apptId)).Tables[0].AsEnumerable()
                .Select(r => new Personal_Appointment
                {
                    SalApptId = r.Field<int>("SalApptId"),
                    CommID = r.Field<string>("CommID"),
                    SalApptStartDate = r.Field<DateTime>("SalApptStartDate").ToShortDateString(),
                    SalApptStartTime = r.Field<string>("SalApptStartTime"),
                    SalApptEndDate = r.Field<DateTime>("SalApptEndDate").ToShortDateString(),
                    SalApptEndTime = r.Field<string>("SalApptEndTime"),
                    ApptDesc = r.Field<string>("ApptDesc"),
                    Notes = r.Field<string>("Notes"),
                    SalApptUserEntered = r.Field<string>("SalApptUserEntered")
                });
                return new ATL_Appointments
                {
                    PersonalAppointments = PersonalAppt.FirstOrDefault()
                };
            }

            return new ATL_Appointments
            {
                ScheduledAppointments = getAppointmentsCollection(Appointment)[0]
            };
       
        }

        internal static string getComboLookupValue(string lookupTable, string lookupColumn, string lookupValueColumn, string lookupValue)
        {
            string result = Convert.ToString(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure,
                                  "spATL_DDL_Lookup",
                                  new SqlParameter("@lookupTable", lookupTable),
                                  new SqlParameter("@lookupColumn", lookupColumn),
                                     new SqlParameter("@lookupValueColumn", lookupValueColumn),
                                        new SqlParameter("@lookupValue", lookupValue)));
            return string.IsNullOrWhiteSpace(result) ? BusinessConstants.NA : result;
        }

        internal static SAL03_ResAppointments getEditAppointment(int apptId)
        {
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
              "spATL_APPT_DETAILS",
               new SqlParameter("@ApptId", apptId)).Tables[0].AsEnumerable();
            return getAppointmentsCollection(dataSet)[0];
        }

        public static string getCommissionName(string CommId)
        {
            //SqlDataReader AssignResult = null;
            var Commission = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_Get_CommName",
               new SqlParameter("@CommId", CommId)).Tables[0];
            string Name = BusinessConstants.NA;
            if (Commission.Rows.Count > 0)
            {
                Name = string.Format("{0} {1}", Convert.ToString(Commission.Rows[0]["SALCommFirstName"]),
                Convert.ToString(Commission.Rows[0]["SALCommLastName"]));
            }

            return Name;
        }

        public static string getUsername(string userId)
        {
            var UserData = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_Get_UserFullName",
                 new SqlParameter("@UserName", userId)).Tables[0];

            string Name = BusinessConstants.NA;
            if (UserData.Rows.Count > 0)
            {
                Name = String.Format("{0} {1}", Convert.ToString(UserData.Rows[0]["SALUserFirstName"]),
                Convert.ToString(UserData.Rows[0]["SALUserLastName"]));
            }
            return Name;
        }

        public static List<SAL10_SalesTypes> getSalesTypeList()
        {
            List<SAL10_SalesTypes> lstSalesType = new List<SAL10_SalesTypes>();

            SqlDataReader SalesTypeResult = null;
            SalesTypeResult = SqlHelper.ExecuteReader(_myConnection, "spddl_SalesType");

            while (SalesTypeResult.Read())
            {
                SAL10_SalesTypes objFenceType = new SAL10_SalesTypes();
                objFenceType.SalTypeId = Convert.ToInt32(SalesTypeResult["SalTypeId"]);
                objFenceType.SalType = Convert.ToString(SalesTypeResult["SalType"]);

                lstSalesType.Add(objFenceType);
            }
            SalesTypeResult.Close();
            return lstSalesType;
        }

        internal static bool SaveAppointment(dynamic modelAppt)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();
                if (modelAppt.SalApptId > 0)
                {
                    parametersList.Add(new SqlParameter("@SalApptId", modelAppt.SalApptId));
                    spName = "spATL_APPT_Upd";
                }
                else
                {
                    spName = "spATL_APPT_Ins";
                }
                parametersList.Add(new SqlParameter("@SalContId", modelAppt.SalContId));
                parametersList.Add(new SqlParameter("@SalApptAddress", modelAppt.SalApptAddress));
                parametersList.Add(new SqlParameter("@SalApptCity", modelAppt.SalApptCity));
                parametersList.Add(new SqlParameter("@SalApptState", modelAppt.SalApptState));
                parametersList.Add(new SqlParameter("@SalApptZip", modelAppt.SalApptZip));
                parametersList.Add(new SqlParameter("@SalApptPhone", Common.FormatPhoneText(modelAppt.SalApptPhone)));
                parametersList.Add(new SqlParameter("@SalApptPhoneExt", Common.FormatPhoneText(modelAppt.SalApptPhoneExt)));
                parametersList.Add(new SqlParameter("@SalApptFax", Common.FormatPhoneText(modelAppt.SalApptFax)));
                parametersList.Add(new SqlParameter("@SalApptMobile", Common.FormatPhoneText(modelAppt.SalApptMobile)));
                parametersList.Add(new SqlParameter("@SalApptEmail", modelAppt.SalApptEmail));
                parametersList.Add(new SqlParameter("@SalApptStartDate", Convert.ToDateTime(modelAppt.SalApptStartDate)));
                parametersList.Add(new SqlParameter("@SalApptStartTime", modelAppt.SalApptStartTime));
                parametersList.Add(new SqlParameter("@SalApptEndDate", Convert.ToDateTime(modelAppt.SalApptEndDate)));
                parametersList.Add(new SqlParameter("@SalApptEndTime", modelAppt.SalApptEndTime));
                parametersList.Add(new SqlParameter("@FenceTypeID", modelAppt.FenceTypeID));
                parametersList.Add(new SqlParameter("@CommID", modelAppt.CommID));
                parametersList.Add(new SqlParameter("@SalTypeId", modelAppt.SalTypeId));
                parametersList.Add(new SqlParameter("@ApptDesc", modelAppt.ApptDesc));
                parametersList.Add(new SqlParameter("@Notes", modelAppt.Notes));
                parametersList.Add(new SqlParameter("@SalApptUserEntered", modelAppt.SalApptUserEntered));

                result = Convert.ToInt32(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()));
            }
            catch (Exception ex)
            {
                result = -2;
            }
            return result > 0;
        }

        internal static bool deleteAppointment(int id)
        {
            int result = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure,
                "spATL_APPT_Del",
                new SqlParameter("@ApptId", id),
                new SqlParameter("@UserName", string.Empty));
            return result > 0;
        }

        internal static List<SAL03_ResAppointments> searchAppointment(string lastName, string city, string zipCode)
        {
            zipCode = string.IsNullOrWhiteSpace(zipCode) ? String.Empty : zipCode;
            var dataSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
              "spATL_APPT_Filter",
              new SqlParameter("@ApptLastName", lastName),
              new SqlParameter("@City", city),
              new SqlParameter("@ZipCode", zipCode)).Tables[0].AsEnumerable();

            return getAppointmentsCollection(dataSet);
        }

        private static List<SAL03_ResAppointments> getAppointmentsCollection(dynamic dataSet)
        {
            List<SAL03_ResAppointments> lstAppointment = new List<SAL03_ResAppointments>();

            foreach (var record in dataSet)
            {
                SAL03_ResAppointments appointment = new SAL03_ResAppointments();
                if (!record["SalApptId"].Equals(DBNull.Value))
                {
                    appointment.SalApptId = Convert.ToInt32(record["SalApptId"]);
                }
                if (!record["SalContId"].Equals(DBNull.Value))
                {
                    appointment.SalContId = Convert.ToInt32(record["SalContId"]);
                }
                if (!record["SalApptFirstName"].Equals(DBNull.Value))
                {
                    appointment.SalApptFirstName = Common.ToTitleCase(Convert.ToString(record["SalApptFirstName"]));
                }
                if (!record["SalApptLastName"].Equals(DBNull.Value))
                {
                    appointment.SalApptLastName = Common.ToTitleCase(Convert.ToString(record["SalApptLastName"]));
                }
                if (!record["SalApptAddress"].Equals(DBNull.Value))
                {
                    appointment.SalApptAddress = Convert.ToString(record["SalApptAddress"]);
                }
                if (!record["SalApptCity"].Equals(DBNull.Value))
                {
                    appointment.SalApptCity = Convert.ToString(record["SalApptCity"]);
                }
                if (!record["SalApptState"].Equals(DBNull.Value))
                {
                    appointment.SalApptState = Convert.ToString(record["SalApptState"]);
                }
                if (!record["SalApptZip"].Equals(DBNull.Value))
                {
                    appointment.SalApptZip = Convert.ToString(record["SalApptZip"]);
                }
                if (record.Table.Columns.Contains("SalApptPhone") && !record["SalApptPhone"].Equals(DBNull.Value))
                {
                    appointment.SalApptPhone = Convert.ToString(record["SalApptPhone"]);
                }
                if (record.Table.Columns.Contains("SalApptPhoneExt") && !record["SalApptPhoneExt"].Equals(DBNull.Value))
                {
                    appointment.SalApptPhoneExt = Convert.ToString(record["SalApptPhoneExt"]);
                }
                if (record.Table.Columns.Contains("SalApptFax") && !record["SalApptFax"].Equals(DBNull.Value))
                {
                    appointment.SalApptFax = Convert.ToString(record["SalApptFax"]);
                }
                if (record.Table.Columns.Contains("SalApptMobile") && !record["SalApptMobile"].Equals(DBNull.Value))
                {
                    appointment.SalApptMobile = Convert.ToString(record["SalApptMobile"]);
                }
                if (record.Table.Columns.Contains("SalApptEmail") && !record["SalApptEmail"].Equals(DBNull.Value))
                {
                    appointment.SalApptEmail = Convert.ToString(record["SalApptEmail"]);
                }
                if (record.Table.Columns.Contains("SalApptStartDate") && !record["SalApptStartDate"].Equals(DBNull.Value))
                {
                    appointment.SalApptStartDate = Convert.ToString(record["SalApptStartDate"]);
                }
                if (record.Table.Columns.Contains("SalApptStartTime") && !record["SalApptStartTime"].Equals(DBNull.Value))
                {
                    appointment.SalApptStartTime = Convert.ToString(record["SalApptStartTime"]);
                }
                if (record.Table.Columns.Contains("SalApptEndDate") && !record["SalApptEndDate"].Equals(DBNull.Value))
                {
                    appointment.SalApptEndDate = Convert.ToString(record["SalApptEndDate"]);
                }
                if (record.Table.Columns.Contains("SalApptEndTime") && !record["SalApptEndTime"].Equals(DBNull.Value))
                {
                    appointment.SalApptEndTime = Convert.ToString(record["SalApptEndTime"]);
                }
                if (record.Table.Columns.Contains("FenceTypeID") && !record["FenceTypeID"].Equals(DBNull.Value))
                {
                    appointment.FenceTypeID = Convert.ToInt32(record["FenceTypeID"]);
                }
                if (record.Table.Columns.Contains("CommID") && !record["CommID"].Equals(DBNull.Value))
                {
                    appointment.CommID = Convert.ToString(record["CommID"]);
                }
                if (record.Table.Columns.Contains("SalTypeId") && !record["SalTypeId"].Equals(DBNull.Value))
                {
                    appointment.SalTypeId = Convert.ToInt32(record["SalTypeId"]);
                }
                if (record.Table.Columns.Contains("Notes") && !record["Notes"].Equals(DBNull.Value))
                {
                    appointment.Notes = Convert.ToString(record["Notes"]);
                }
                if (record.Table.Columns.Contains("SalApptUserEntered") && !record["SalApptUserEntered"].Equals(DBNull.Value))
                {
                    appointment.SalApptUserEntered = Convert.ToString(record["SalApptUserEntered"]);

                }
                lstAppointment.Add(appointment);
            }

            return lstAppointment;
        }

        public static List<AssignTo> getAssignToList()
        {
            //SqlDataReader AssignResult = null;
            var AssignResult = SqlHelper.ExecuteDataset(_myConnection, "spddl_AssignTo").Tables[0].AsEnumerable().Select(r => new AssignTo
            {
                CommID = r.Field<string>("CommID"),
                EmployeeID = r.Field<string>("EmployeeID"),
                FullName = String.Format("{0} {1}", r.Field<string>("LastName"), r.Field<string>("FirstName"))
            });
            return AssignResult.ToList<AssignTo>();
        }

        public static List<SAL04_ApptDesc> getAppointmentDescriptions()
        {
            //SqlDataReader AssignResult = null;
            var Descriptions = SqlHelper.ExecuteDataset(_myConnection, "spddl_ApptDesc").Tables[0].AsEnumerable().Select(r => new SAL04_ApptDesc
            {
                SALApptDescId = r.Field<int>("SALApptDescId"),
                SALApptDesc = r.Field<string>("SALApptDesc"),
            });
            return Descriptions.ToList<SAL04_ApptDesc>();
        }

        public static List<Calendar_Evts> getCalendarEvents(string CommId)
        {
            var CalEvents = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
            "spATL_CAL_APPT_Details",
            new SqlParameter("@commId", CommId)).Tables[0].AsEnumerable();

            List<Calendar_Evts> lstEvents = new List<Calendar_Evts>();

            foreach (var record in CalEvents)
            {
                Calendar_Evts events = new Calendar_Evts();
                if (!record["id"].Equals(DBNull.Value))
                {
                    events.id = Convert.ToInt64(record["id"]);
                }
                if (!record["allDay"].Equals(DBNull.Value))
                {
                    events.allDay = Convert.ToString(record["allDay"]);
                }
                if (!record["title"].Equals(DBNull.Value))
                {
                    events.title = Convert.ToString(record["title"]);
                }
                if (!record["startDate"].Equals(DBNull.Value))
                {
                    var dtTime = Convert.ToDateTime(record["startDate"]);
                    var time = Convert.ToString(record["StartTime"]);
                    events.start = (Convert.ToDateTime(Convert.ToDateTime(record["startDate"]).ToShortDateString() + " " + time)).AddHours(-4);
                }
                if (!record["EndDate"].Equals(DBNull.Value))
                {
                    var dtTime = Convert.ToDateTime(record["EndDate"]);
                    var time = Convert.ToString(record["EndTime"]);
                    events.end = (Convert.ToDateTime(Convert.ToDateTime(record["EndDate"]).ToShortDateString() + " " + time)).AddHours(-4);
                }
                if (!record["color"].Equals(DBNull.Value))
                {
                    events.color = Convert.ToString(record["color"]);
                }
                lstEvents.Add(events);

            }
            return lstEvents;
        }

        public static List<Setup40_Time> getTime()
        {
            List<Setup40_Time> lstTime = new List<Setup40_Time>();

            SqlDataReader TimeResult = null;
            TimeResult = SqlHelper.ExecuteReader(_myConnection, "spddl_Time");

            while (TimeResult.Read())
            {
                Setup40_Time objTime = new Setup40_Time();
                objTime.Setup40TimeID = Convert.ToInt32(TimeResult["Setup40TimeID"]);
                objTime.Setup40Time = Convert.ToString(TimeResult["Setup40Time"]);

                lstTime.Add(objTime);
            }
            TimeResult.Close();
            return lstTime;
        }
    }

    public class AssignTo
    {
        public string CommID { get; set; }
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
    }

    public class SAL04_ApptDesc
    {
        public int SALApptDescId { get; set; }
        public string SALApptDesc { get; set; }
    }

    public class Calendar_Evts
    {
        public Int64 id { get; set; }
        public string allDay { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string color { get; set; }
    }

}


