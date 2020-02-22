using Atlas.DAL;
using Atlas.Models;
using Atlas.Models.DBO;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Atlas.DataAccess.Entity
{
    public static class HomeDal
    {
        private static string _myConnection;

        static HomeDal()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        internal static List<Timesheet> getCrewTimesheet(string code, string bidid)
        {
            string spName = string.Empty;
            if (code == "1")
            {
                spName = "spATL_LBID_CrewTimeSheet";
            }
            else if (code == "2")
            {
                spName = "spATL_LBID_SubTimeSheet";
            }

            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, spName,
                                   new SqlParameter("@BIDID", bidid)).Tables[0].AsEnumerable();
            List<Timesheet> lstTimesheet = new List<Timesheet>();
            foreach (DataRow item in data)
            {
                Timesheet ts = new Timesheet();
                ts.FormName = Convert.ToString(item["FormName"]);
                ts.Company = Convert.ToString(item["Company"]);
                ts.MhRate = Convert.ToString(item["MhRate"]);
                ts.JobNumber = Convert.ToString(item["JobNumber"]);
                ts.JobName = Convert.ToString(item["JobName"]);
                ts.BIDID = Convert.ToInt32(item["BIDID"]);
                ts.Location = Convert.ToString(item["Location"]);
                ts.LaborId = Convert.ToInt32(item["LaborId"]);
                ts.LaborDescription = Convert.ToString(item["LaborDescription"]);
                ts.BidQty = Convert.ToString(item["BidQty"]);
                ts.Uom = Convert.ToString(item["Uom"]);
                ts.MhsEa = Convert.ToDecimal(item["MhsEa"]);
                ts.MhsExt = Convert.ToDecimal(item["MhsExt"]);
                ts.RateEa = Convert.ToDecimal(item["RateEa"]);
                ts.PayExt = Math.Round(Convert.ToDecimal(item["PayExt"]), 2);
                ts.Sort = Convert.ToString(item["Sort"]);
                lstTimesheet.Add(ts);
            }
            return lstTimesheet;
        }

        public static Projects getProjects(string name, string city, string CommID)
        {
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_PRJ_Filter",
                                    new SqlParameter("@ProjName", name),
                                    new SqlParameter("@JobSiteCity", city),
                                    new SqlParameter("@CommId", CommID));

            Projects projects = new Projects();
            projects.QuotedProjects = new List<ProjectView>();
            projects.ActivePendingProjects = new List<ProjectView>();
            projects.CompletedProjects = new List<ProjectView>();

            foreach (DataRow item in data.Tables[0].Rows)
            {
                ProjectView pv = new ProjectView();
                pv.PRJID = Convert.ToInt32(item["PRJID"]);
                pv.JobStatus = Convert.ToString(item["JobStatusId"]).ToUpper();
                pv.ProjectName = Convert.ToString(item["ProjectName"]);
                pv.JobSiteCity = Convert.ToString(item["JobSiteCity"]);
                projects.QuotedProjects.Add(pv);
            }
            foreach (DataRow item in data.Tables[1].Rows)
            {
                ProjectView pv = new ProjectView();
                pv.PRJID = Convert.ToInt32(item["PRJID"]);
                pv.JobStatus = Convert.ToString(item["JobStatusId"]);
                pv.ProjectName = Convert.ToString(item["ProjectName"]);
                pv.JobSiteCity = Convert.ToString(item["JobSiteCity"]);
                projects.ActivePendingProjects.Add(pv);
            }
            foreach (DataRow item in data.Tables[2].Rows)
            {
                ProjectView pv = new ProjectView();
                pv.PRJID = Convert.ToInt32(item["PRJID"]);
                pv.JobStatus = Convert.ToString(item["JobStatusId"]);
                pv.ProjectName = Convert.ToString(item["ProjectName"]);
                pv.JobSiteCity = Convert.ToString(item["JobSiteCity"]);
                projects.CompletedProjects.Add(pv);
            }
            return projects;
        }

        internal static PreBidModel getPreBidFormByBid(string id)
        {
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_LBID_Prebid",
                                   new SqlParameter("@BIDID", id)).Tables[0].AsEnumerable();


            DataRow dr = data.FirstOrDefault();
            PreBidModel prebid = new PreBidModel
            {
                FormType = Convert.ToString(dr["FormType"]),
                JobNumber = Convert.ToString(dr["JobNumber"]),
                JobName = Convert.ToString(dr["JobName"]),
                BINumber = Convert.ToInt32(dr["BINumber"]),
                Company = Convert.ToString(dr["Company"]),
                BIName = Convert.ToString(dr["BIName"]),
                SalesPerson = Convert.ToString(dr["SalesPerson"]),
                FenceType = Convert.ToString(dr["FenceType"]),
                RateType = Convert.ToString(dr["RateType"]),
                Material = Math.Round(Convert.ToDecimal(dr["Material"]), 2),
                MaterialHandling = Math.Round(Convert.ToDecimal(dr["MaterialHandling"]), 2),
                Concrete = Math.Round(Convert.ToDecimal(dr["Concrete"]), 2),
                MaterialTotal = Math.Round(Convert.ToDecimal(dr["MaterialTotal"]), 2),
                MaterialCOSPercent = Math.Round(Convert.ToDecimal(dr["MaterialCOSPercent"]), 2),
                OnsiteLabor = Math.Round(Convert.ToDecimal(dr["OnsiteLabor"]), 2),
                LoadLabor = Math.Round(Convert.ToDecimal(dr["LoadLabor"]), 2),
                DriveLabor = Math.Round(Convert.ToDecimal(dr["DriveLabor"]), 2),
                Supervisor = Math.Round(Convert.ToDecimal(dr["Supervisor"]), 2),
                LaborReserve = Math.Round(Convert.ToDecimal(dr["LaborReserve"]), 2),
                LaborTotal = Math.Round(Convert.ToDecimal(dr["LaborTotal"]), 2),
                LaborCOSPercent = Math.Round(Convert.ToDecimal(dr["LaborCOSPercent"]), 2),
                OtherCharges = Math.Round(Convert.ToDecimal(dr["OtherCharges"]), 2),
                OtherChargesMarkup = Math.Round(Convert.ToDecimal(dr["OtherChargesMarkup"]), 2),
                OtherChargesTotal = Math.Round(Convert.ToDecimal(dr["OtherChargesTotal"]), 2),
                OtherCOSPercent = Math.Round(Convert.ToDecimal(dr["OtherCOSPercent"]), 2),
                EquipmentCost = Math.Round(Convert.ToDecimal(dr["EquipmentCost"]), 2),
                EquipmentCOSPercent = Math.Round(Convert.ToDecimal(dr["EquipmentCOSPercent"]), 2),
                Benefits = Math.Round(Convert.ToDecimal(dr["Benefits"]), 2),
                Retirement = Math.Round(Convert.ToDecimal(dr["Retirement"]), 2),
                PayrollTax = Math.Round(Convert.ToDecimal(dr["PayrollTax"]), 2),
                WorkersComp = Math.Round(Convert.ToDecimal(dr["WorkersComp"]), 2),
                IndirectTotal = Math.Round(Convert.ToDecimal(dr["IndirectTotal"]), 2),
                IndirectCOSPercent = Math.Round(Convert.ToDecimal(dr["IndirectCOSPercent"]), 2),
                JobCost = Math.Round(Convert.ToDecimal(dr["JobCost"]), 2),
                JobMarkupPercent = Math.Round(Convert.ToDecimal(dr["JobMarkupPercent"]), 2),
                JobMarkUpTotal = Math.Round(Convert.ToDecimal(dr["JobMarkUpTotal"]), 2),
                SuggestdSoldFor = Math.Round(Convert.ToDecimal(dr["SuggestdSoldFor"]), 2),
                PreTaxSoldFor = Math.Round(Convert.ToDecimal(dr["PreTaxSoldFor"]), 2),
                SalesTaxType = Convert.ToString(dr["SalesTaxType"]),
                SalesTaxPercent = Math.Round(Convert.ToDecimal(dr["SalesTaxPercent"]), 2),
                SalesTaxTotal = Math.Round(Convert.ToDecimal(dr["SalesTaxTotal"]), 2),
                CrewHeadCount = Convert.ToString(dr["CrewHeadCount"]),
                DaysOnsite = Math.Round(Convert.ToDecimal(dr["DaysOnsite"]), 2),
                CrewLaborBudget = Math.Round(Convert.ToDecimal(dr["CrewLaborBudget"]), 2),
                RevPerMh = Math.Round(Convert.ToDecimal(dr["RevPerMh"]), 2),
                PRJID = Convert.ToString(dr["PRJID"]),
            };
            return prebid;

        }

        internal static PreBidModel getPreBidFormByProject(string id)
        {
            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_PRJ_Prebid",
                                   new SqlParameter("@PRJID", id)).Tables[0].AsEnumerable();


            DataRow dr = data.FirstOrDefault();
            PreBidModel prebid = new PreBidModel
            {
                FormType = Convert.ToString(dr["FormType"]),
                JobNumber = Convert.ToString(dr["JobNumber"]),
                JobName = Convert.ToString(dr["JobName"]),
                Company = Convert.ToString(dr["Company"]),
                SalesPerson = Convert.ToString(dr["SalesPerson"]),
                RateType = Convert.ToString(dr["RateType"]),
                Material = Math.Round(Convert.ToDecimal(dr["Material"]), 2),
                MaterialHandling = Math.Round(Convert.ToDecimal(dr["MaterialHandling"]), 2),
                Concrete = Math.Round(Convert.ToDecimal(dr["Concrete"]), 2),
                MaterialTotal = Math.Round(Convert.ToDecimal(dr["MaterialTotal"]), 2),
                OnsiteLabor = Math.Round(Convert.ToDecimal(dr["OnsiteLabor"]), 2),
                LoadLabor = Math.Round(Convert.ToDecimal(dr["LoadLabor"]), 2),
                DriveLabor = Math.Round(Convert.ToDecimal(dr["DriveLabor"]), 2),
                Supervisor = Math.Round(Convert.ToDecimal(dr["Supervisor"]), 2),
                LaborReserve = Math.Round(Convert.ToDecimal(dr["LaborReserve"]), 2),
                LaborTotal = Math.Round(Convert.ToDecimal(dr["LaborTotal"]), 2),
                OtherCharges = Math.Round(Convert.ToDecimal(dr["OtherCharges"]), 2),
                OtherChargesMarkup = Math.Round(Convert.ToDecimal(dr["OtherChargesMarkup"]), 2),
                OtherChargesTotal = Math.Round(Convert.ToDecimal(dr["OtherChargesTotal"]), 2),
                EquipmentCost = Math.Round(Convert.ToDecimal(dr["EquipmentCost"]), 2),
                Benefits = Math.Round(Convert.ToDecimal(dr["Benefits"]), 2),
                Retirement = Math.Round(Convert.ToDecimal(dr["Retirement"]), 2),
                PayrollTax = Math.Round(Convert.ToDecimal(dr["PayrollTax"]), 2),
                WorkersComp = Math.Round(Convert.ToDecimal(dr["WorkersComp"]), 2),
                IndirectTotal = Math.Round(Convert.ToDecimal(dr["IndirectTotal"]), 2),
                JobCost = Math.Round(Convert.ToDecimal(dr["JobCost"]), 2),
                JobMarkUpTotal = Math.Round(Convert.ToDecimal(dr["JobMarkUpTotal"]), 2),
                SuggestdSoldFor = Math.Round(Convert.ToDecimal(dr["SuggestdSoldFor"]), 2),
                PreTaxSoldFor = Math.Round(Convert.ToDecimal(dr["PreTaxSoldFor"]), 2),
                SalesTaxTotal = Math.Round(Convert.ToDecimal(dr["SalesTaxTotal"]), 2),
                DaysOnsite = Math.Round(Convert.ToDecimal(dr["DaysOnsite"]), 2),
                CrewLaborBudget = Math.Round(Convert.ToDecimal(dr["CrewLaborBudget"]), 2),
                PRJID = Convert.ToString(dr["PRJID"]),
            };
            return prebid;

        }

        internal static int UpdateSalesTax(string bidid, string salexTax)
        {
            SqlHelper.ExecuteScalar(_myConnection, CommandType.Text, "UPDATE[dbo].[BID01_Headers] SET [PreTxSoldFor] = @tax WHERE[BIDID] = @Bidid",
                new SqlParameter("@Bidid", bidid),
                new SqlParameter("@tax", salexTax));

            return 0;
        }

        internal static List<Packaging> getPackaging(string bidid)
        {

            var data = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure, "spATL_LBID_MRO",
                                   new SqlParameter("@BIDID", bidid)).Tables[0].AsEnumerable();
            List<Packaging> lstPackaging = new List<Packaging>();
            foreach (DataRow item in data)
            {
                Packaging mro = new Packaging();
                mro.MRO_Number = Convert.ToInt32(item["MRO_Number"]);
                mro.CfsFileName = Convert.ToString(item["CfsFileName"]);
                mro.RunDate = Convert.ToDateTime(item["RunDate"]);
                mro.JobNumber = item["JobNumber"] != null ? Convert.ToString(item["JobNumber"]) : "";
                mro.ProjectName = Convert.ToString(item["ProjectName"]);
                mro.BidItemNumber = Convert.ToInt32(item["BidItemNumber"]);
                mro.BIDName = Convert.ToString(item["BIDName"]);
                mro.Qty = Convert.ToDecimal(item["Qty"]);
                mro.PartNum = Convert.ToString(item["PartNum"]);
                mro.Description = Convert.ToString(item["Description"]);
                lstPackaging.Add(mro);
            }
            return lstPackaging;
        }

    }
}