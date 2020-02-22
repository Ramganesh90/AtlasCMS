using Atlas.DAL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Atlas.Models.DBO;
using System.Configuration;
using Atlas.Models;

namespace Atlas.DataAccess.Entity
{

    public static class AdminDal
    {
        private static string _myConnection;

        static AdminDal()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        internal static List<PRL01_Employees> getSalesman()
        {
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spddl_Salesmen")
                                        .Tables[0].AsEnumerable();
            List<PRL01_Employees> lstEmployees = new List<PRL01_Employees>();
            foreach (DataRow item in data)
            {
                var source = new PRL01_Employees
                {
                    FirstName = Convert.ToString(item["FirstName"]),
                    LastName = Convert.ToString(item["LastName"]),
                    EmployeeID = Convert.ToString(item["CommID"])
                };
                lstEmployees.Add(source);
            }
            return lstEmployees;
        }

        internal static List<SearchPunchListModel> searchPunchList(string projectName, string city)
        {
            dynamic data = null;

                string spName = "spATL_ADM_GenPunchlist_Search";
                data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, spName,
                     new SqlParameter("@ProjectName", projectName),
                     new SqlParameter("@JobSiteCity", city)).Tables[0].AsEnumerable();
            List<SearchPunchListModel> lstProjects = new List<SearchPunchListModel>();
            foreach (DataRow item in data)
            {
                var source = new SearchPunchListModel();
                source.JobName = Convert.ToString(item["ProjectName"]);
                source.JobNumber = Convert.ToString(item["JobNumber"]);
                source.PRJID = Convert.ToInt32(item["PRJID"]);
                source.FirstName = Convert.ToString(item["BillingFirstName"]);
                source.LastName = Convert.ToString(item["BillingLastName"]);
                source.JobAddress = Convert.ToString(item["JobSiteAddress"]);
                source.JobCity = Convert.ToString(item["JobSiteCity"]);
                source.JobState = Convert.ToString(item["JobSiteState"]);
                source.JobZip = Convert.ToString(item["JobSiteZip"]);
                source.Salesmen = Convert.ToString(item["Salesman"]);
                var BidData = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_ADM_GenPunchlist_Search_BI",
                        new SqlParameter("@PRJID", source.PRJID)).Tables[0].AsEnumerable();
                source.BidItem = new List<BidItems>();
                foreach (DataRow bid in BidData)
                {
                    var row = new BidItems
                    {
                        BidItemId = Convert.ToString(bid["BIDID"]),
                        BidItemName = Convert.ToString(bid["BIDName"]),
                        FenceType = Convert.ToString(bid["FenceType"])
                    };
                    source.BidItem.Add(row);
                }
                lstProjects.Add(source);
            }
            return lstProjects;
        }

        internal static int saveComplaint(Complaints modelComplaint,string Name)
        {
            var result = Convert.ToInt16(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_ADM_GenPunchlist_Ins",
                        new SqlParameter("@PRJID", modelComplaint.PRJID),
                        new SqlParameter("@Notes", modelComplaint.JobNotes),
                        new SqlParameter("@UserName",Name)));

            return result;
        }

        internal static int UpdateProject(int id, string action)
        {
            int result = -2;
            try
            {
                string spName = string.Empty;
                switch (action)
                {
                    case "jobComplete":
                        spName = "spATL_ADM_PRJ_Completed";
                        break;
                    case "jobActivate":
                        spName = "spATL_ADM_PRJ_Activate";
                        break;
                    case "jobDeactivate":
                        spName = "spATL_ADM_PRJ_DeActivate";
                        break;
                    default: break;
                }
                result = Convert.ToInt16(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, spName,
                        new SqlParameter("@PRJID", id)));

            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static int UpdateBid(int id, string action)
        {
            int result = -2;
            try
            {
                string spName = string.Empty;
                switch (action)
                {
                    case "bidComplete":
                        spName = "spATL_ADM_BID_Completed";
                        break;
                    case "bidActivate":
                        spName = "spATL_ADM_BID_Activate";
                        break;
                    case "bidDeactivate":
                        spName = "spATL_ADM_BID_DeActivate";
                        break;
                    case "bidEditActivate":
                        spName = "spATL_ADM_BID_EditBIActivation";
                        break;

                    default: break;
                }
                result = Convert.ToInt16(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, spName,
                        new SqlParameter("@BIDID", id)));

            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static int UpdatePunchListStatus(int punchId, string status)
        {
            int result = -2;
            try
            {
                string spName = string.Empty;
                result = Convert.ToInt16(SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_ADM_Punchlist_Upd",
                        new SqlParameter("@CMSId", punchId),
                        new SqlParameter("@CmsStatusId",status)
                        ));

            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static int saveMaintenance(Contract modelContract)
        {
            int result = -2;
            try
            {
                string spName = string.Empty;
                var id= SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_ADM_JobBillDtMain_PRJ_Upd",
                        new SqlParameter("@PRJID", modelContract.PRJID),
                        new SqlParameter("@ContractDate", modelContract.ContractDate),
                        new SqlParameter("@ContractorsProjectNumber", modelContract.ContractorsProjectNumber),
                        new SqlParameter("@RetainageCompletedWork", modelContract.RetainageCompletedWork),
                        new SqlParameter("@RetainageStoredMaterial", modelContract.RetainageStoredMaterial));

                result = id == DBNull.Value ? 1 : result;
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static List<PunchStatus> PunchStatus()
        {
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_ddl_PunchlistStatus")
                                    .Tables[0].AsEnumerable();
            var lstPunchStatus = new List<PunchStatus>();
            foreach (DataRow item in data)
            {
                var source = new PunchStatus
                {
                    CmsStatusId = Convert.ToInt32(item["CmsStatusId"]),
                    StatusName = Convert.ToString(item["StatusName"]),
                };
                lstPunchStatus.Add(source);
            }
            return lstPunchStatus;
        }

        internal static int saveAiaBilling(AIABilling modelBilling)
        {
            int result = -2;
            try
            {
                string spName = string.Empty;
                var id =  (SqlHelper.ExecuteScalar(_myConnection, CommandType.StoredProcedure, "spATL_ADM_JobBillDtMain_AIA_Ins",
                        new SqlParameter("@PRJID", modelBilling.PRJID),
                        new SqlParameter("@CommissionExpires", modelBilling.CommissionExpires),
                        new SqlParameter("@ApplicationNum", modelBilling.ApplicationNum),
                        new SqlParameter("@BillingDate", modelBilling.BillingDate),
                        new SqlParameter("@EntryDate", modelBilling.EntryDate),
                        new SqlParameter("@NotaryDate", modelBilling.NotaryDate),
                        new SqlParameter("@OriginalContractSum", modelBilling.OriginalContractSum),
                        new SqlParameter("@NetChangeByCO", modelBilling.NetChangeByCO),
                        new SqlParameter("@ItemNo", modelBilling.ItemNo),
                        new SqlParameter("@QtyOfBI", modelBilling.QtyOfBI),
                        new SqlParameter("@unitOfMeasure", modelBilling.UnitOfMeasure),
                        new SqlParameter("@UnitPrice", modelBilling.UnitPrice),
                        new SqlParameter("@BIDID", modelBilling.BIDID),
                        new SqlParameter("@BilledQty", modelBilling.BilledQty),
                        new SqlParameter("@Accecpt", modelBilling.Accept)));

                result = id == DBNull.Value ? 1 : result;
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static Complaints getComplaintData(int prjId)
        {
            string spName = "spATL_ADM_GenPunchlist_Search_PRJID";
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, spName,
                 new SqlParameter("@PRJID", prjId)).Tables[0].AsEnumerable(); //jobno
            List<Complaints> lstComplaints = new List<Complaints>();
            foreach (DataRow item in data)
            {
                var source = new Complaints();
                source.JobName = Convert.ToString(item["ProjectName"]);
                source.JobNumber = Convert.ToString(item["JobNumber"]);
                source.PRJID = Convert.ToInt32(item["PRJID"]);
                source.JobAddress = Convert.ToString(item["JobSiteAddress"]);
                source.JobPhone = Convert.ToString(item["JobSitePhone"]);
                source.JobContact = Convert.ToString(item["JobSiteMobilePhone"]);
                source.JobCity = Convert.ToString(item["JobSiteCity"]);
                source.Salesman = Convert.ToString(item["Salesman"]);
                lstComplaints.Add(source);
            }
            return lstComplaints.FirstOrDefault();
        }

        internal static List<PunchList> ViewPunchlist(string role, string user)
        {
            var lstPunchStatus = new List<PunchStatus>();
            if (CheckUserStatus(role,user))
            {
                lstPunchStatus = PunchStatus();
            }

            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_ADM_ViewPunchlist")
                                      .Tables[0].AsEnumerable();
            var lstPunchList = new List<PunchList>();
            foreach (DataRow item in data)
            {
                var source = new PunchList
                {
                    PunchListId = Convert.ToString(item["CmsId"]),
                    JobNumber = Convert.ToString(item["JobNumber"]),
                    JobName = Convert.ToString(item["ProjectName"]),
                    Problem = Convert.ToString(item["Description"]),
                    Status = Convert.ToString(item["CmsStatusID"]),
                    DateReceived = Convert.ToDateTime(item["DateRec"]).ToShortDateString(),
                    PunchStatuses = lstPunchStatus
                };
                lstPunchList.Add(source);
            }
            return lstPunchList;
            
        }

        internal static AIABilling getAIABilling(string PRJID, object BIDID)
        {
            AIABilling modelBilling = null;
            try
            {

                var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_ADM_JobBillDtMain_AIA_Get_Dtls",
                    new SqlParameter("@PRJID",PRJID),
                    new SqlParameter("@BIDID",BIDID));


                if (data.Tables[0].Rows.Count == 1)
                {
                    var result = data.Tables[0];
                    modelBilling = new AIABilling
                    {
                        CommissionExpires = Convert.ToDateTime(result.Rows[0]["CommissionExpires"]).Date,
                        ApplicationNum = Convert.ToInt32(result.Rows[0]["ApplicationNum"]),
                        BillingDate = Convert.ToDateTime(result.Rows[0]["BillingDate"]),
                        EntryDate = Convert.ToDateTime(result.Rows[0]["EntryDate"]),
                        NotaryDate = Convert.ToDateTime(result.Rows[0]["NotaryDate"]),
                        OriginalContractSum = Convert.ToInt32(result.Rows[0]["OriginalContractSum"]),
                        NetChangeByCO = Convert.ToInt32(result.Rows[0]["NetChangeByCO"]),
                        ItemNo = Convert.ToString(result.Rows[0]["ItemNo"]),
                        QtyOfBI = Convert.ToInt32(result.Rows[0]["QtyOfBI"]),
                        UnitOfMeasure = Convert.ToString(result.Rows[0]["UnitOfMeasure"]),
                        UnitPrice = Convert.ToString(result.Rows[0]["UnitPrice"]),
                        BilledQty = Convert.ToInt32(result.Rows[0]["BilledQty"]),
                        Accept = Convert.ToBoolean(result.Rows[0]["Accept"]),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return modelBilling;
        }

        private static bool CheckUserStatus(string role,string user)
        {
            var UserKey = Convert.ToString(ConfigurationManager.AppSettings["punchUsers"])?.Split(';');
            var RoleKey = Convert.ToString(ConfigurationManager.AppSettings["punchRole"])?.Split(';');

            return (UserKey.Any(i=> i.Equals(user, StringComparison.OrdinalIgnoreCase))) || 
                RoleKey.Any(i => i.Equals(role, StringComparison.OrdinalIgnoreCase));

        }
    }
}