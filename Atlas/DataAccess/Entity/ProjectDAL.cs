using Atlas.DAL;
using Atlas.Models;
using Atlas.Models.DBO;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Atlas.DataAccess.Entity
{
    public class ProjectDAL
    {
        private static string _myConnection;

        static ProjectDAL()
        {
            DataObject dataObject = new DataObject();
            _myConnection = dataObject.getConnection();
        }

        internal static DataSet getRateTypes(string commId)
        {
            DataSet result = null;
            try
            {
                result = SqlHelper.ExecuteDataset(_myConnection, "spATL_PRJ",
                    new SqlParameter("@CommId", commId));
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        internal static ProjectViewModel getProjectFormDetails(int PRJID)
        {
            var result = SqlHelper.ExecuteDataset(_myConnection, "spATL_PRJ_GetPRJDetails",
                new SqlParameter("@PRJID", PRJID)).Tables[0];

            var pvm = new ProjectViewModel();
            pvm.JobSites = new PRJ04_JobSites();
            pvm.BillingInfoDetails = new PRJ08_BillingInfo();
            pvm.JobDetails = new PRJ01_Headers();
            if (result.Rows.Count > 0)
            {
                pvm.JobSites.JobSiteId = result.Rows[0]["JobSiteId"] != DBNull.Value ? Convert.ToInt32(result.Rows[0]["JobSiteId"]) : 0;
                pvm.JobSites.IsCommercial = result.Rows[0]["IsCommercial"] != DBNull.Value ? Convert.ToBoolean(result.Rows[0]["IsCommercial"]) : false;
                pvm.JobSites.JobName = Convert.ToString(result.Rows[0]["JobName"]);
                pvm.JobSites.BillingFirstName = Convert.ToString(result.Rows[0]["BillingFirstName"]);
                pvm.JobSites.BillingLastName = Convert.ToString(result.Rows[0]["BillingLastName"]);
                pvm.JobSites.BillingCompanyName = Convert.ToString(result.Rows[0]["BillingCompanyName"]);
                pvm.JobSites.JobSiteAddress = Convert.ToString(result.Rows[0]["JobSiteAddress"]);
                pvm.JobSites.JobSiteCity = Convert.ToString(result.Rows[0]["JobSiteCity"]);
                pvm.JobSites.JobSiteState = Convert.ToString(result.Rows[0]["JobSiteState"]);
                pvm.JobSites.JobSiteZip = Convert.ToString(result.Rows[0]["JobSiteZip"]);
                pvm.JobSites.JobSitePhone = Convert.ToString(result.Rows[0]["JobSitePhone"]);
                pvm.JobSites.JobSitePhoneExt = Convert.ToString(result.Rows[0]["JobSitePhoneExt"]);
                pvm.JobSites.JobSiteFax = Convert.ToString(result.Rows[0]["JobSiteFax"]);
                pvm.JobSites.JobSiteMobilePhone = Convert.ToString(result.Rows[0]["JobSiteMobilePhone"]);
                pvm.JobSites.JobSiteEMail = Convert.ToString(result.Rows[0]["JobSiteEMail"]);
                pvm.JobSites.SalApptId = result.Rows[0]["SalApptId"] != DBNull.Value ? Convert.ToInt32(result.Rows[0]["SalApptId"]) : 0;
                pvm.JobSites.SalContId = result.Rows[0]["SalContId"] != DBNull.Value ? Convert.ToInt32(result.Rows[0]["SalContId"]) : 0;

                pvm.BillingInfoDetails.PRJBillingID = Convert.ToInt32(result.Rows[0]["PRJBillingID"]);
                pvm.BillingInfoDetails.BillingFirstName = Convert.ToString(result.Rows[0]["BillingFirstName"]);
                pvm.BillingInfoDetails.BillingLastName = Convert.ToString(result.Rows[0]["BillingLastName"]);
                pvm.BillingInfoDetails.BillingCompanyName = Convert.ToString(result.Rows[0]["BillingCompanyName"]);
                pvm.BillingInfoDetails.BillingAddress = Convert.ToString(result.Rows[0]["BillingAddress"]);
                pvm.BillingInfoDetails.BillingCity = Convert.ToString(result.Rows[0]["BillingCity"]);
                pvm.BillingInfoDetails.BillingState = Convert.ToString(result.Rows[0]["BillingState"]);
                pvm.BillingInfoDetails.BillingZip = Convert.ToString(result.Rows[0]["BillingZip"]);
                pvm.BillingInfoDetails.BillingPhone = Convert.ToString(result.Rows[0]["BillingContPhone"]);
                pvm.BillingInfoDetails.BillingPhoneExt = Convert.ToString(result.Rows[0]["BillingContPhoneExt"]);
                pvm.BillingInfoDetails.BillingFax = Convert.ToString(result.Rows[0]["BillingFax"]);
                pvm.BillingInfoDetails.BillingMobilePhone = Convert.ToString(result.Rows[0]["BillingMobilePhone"]);
                pvm.BillingInfoDetails.BillingEMail = Convert.ToString(result.Rows[0]["BillingEMail"]);

                pvm.JobDetails.PRJID = Convert.ToInt32(result.Rows[0]["PRJID"]);
                pvm.JobDetails.PRJDateEntered = Convert.ToDateTime(result.Rows[0]["PRJDateEntered"]).Date;
                pvm.JobDetails.DivID = Convert.ToString(result.Rows[0]["DivID"]);
                pvm.JobDetails.MhRateID = Convert.ToString(result.Rows[0]["MhRateID"]);
                pvm.JobDetails.JobStatusId = Convert.ToString(result.Rows[0]["JobStatusId"]);
                pvm.JobDetails.DriveTime1Way = Convert.ToDecimal(result.Rows[0]["DriveTime1Way"]);
                pvm.JobDetails.PRJNotes = Convert.ToString(result.Rows[0]["PRJNotes"]);
            }
            return pvm;
        }

        internal static int saveJobSiteInformation(PRJ04_JobSites modelJobInfo)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();

                if (modelJobInfo.JobSiteId == 0)
                {
                    spName = "spATL_PRJ_JBSite_Ins";
                }
                else
                {
                    spName = "spATL_PRJ_JBSite_Upd";
                    parametersList.Add(new SqlParameter("@JobSiteID", modelJobInfo.JobSiteId));
                }

                parametersList.Add(new SqlParameter("@JobName", modelJobInfo.JobName));
                parametersList.Add(new SqlParameter("@JS_FirstName", modelJobInfo.BillingFirstName));
                parametersList.Add(new SqlParameter("@JS_LastName", modelJobInfo.BillingLastName));
                parametersList.Add(new SqlParameter("@JS_CompanyName", modelJobInfo.BillingCompanyName));
                parametersList.Add(new SqlParameter("@JS_Address", modelJobInfo.JobSiteAddress));
                parametersList.Add(new SqlParameter("@JS_City", modelJobInfo.JobSiteCity));
                parametersList.Add(new SqlParameter("@JS_State", modelJobInfo.JobSiteState));
                parametersList.Add(new SqlParameter("@JS_Zip", modelJobInfo.JobSiteZip));
                parametersList.Add(new SqlParameter("@JS_Phone", modelJobInfo.JobSitePhone));
                parametersList.Add(new SqlParameter("@JS_PhoneExt", modelJobInfo.JobSitePhoneExt));
                parametersList.Add(new SqlParameter("@JS_Fax", modelJobInfo.JobSiteFax));
                parametersList.Add(new SqlParameter("@JS_Mobile", modelJobInfo.JobSiteMobilePhone));
                parametersList.Add(new SqlParameter("@JS_Email", modelJobInfo.JobSiteEMail));
                parametersList.Add(new SqlParameter("@SalApptId", modelJobInfo.SalApptId > 0 ? modelJobInfo.SalApptId : null));
                parametersList.Add(new SqlParameter("@SalContId", modelJobInfo.SalContId > 0 ? modelJobInfo.SalContId : null));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);
                result = resultSet.Rows[0][0] == DBNull.Value ? modelJobInfo.JobSiteId : Convert.ToInt32(resultSet.Rows[0][0]);
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }

        internal static int saveBillingInformation(PRJ08_BillingInfo modelBilling)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();
                if (modelBilling.PRJBillingID > 0)
                {
                    parametersList.Add(new SqlParameter("@PRJBillingID", modelBilling.PRJBillingID));
                    spName = "spATL_PRJ_Billing_Upd";
                }
                else
                {
                    spName = "spATL_PRJ_Billing_Ins";
                }

                parametersList.Add(new SqlParameter("@Bill_FirstName", modelBilling.BillingFirstName));
                parametersList.Add(new SqlParameter("@Bill_LastName", modelBilling.BillingLastName));
                parametersList.Add(new SqlParameter("@Bill_CompanyName", modelBilling.BillingCompanyName));
                parametersList.Add(new SqlParameter("@Bill_Address", modelBilling.BillingAddress));
                parametersList.Add(new SqlParameter("@Bill_City", modelBilling.BillingCity));
                parametersList.Add(new SqlParameter("@Bill_State", modelBilling.BillingState));
                parametersList.Add(new SqlParameter("@Bill_Zip", modelBilling.BillingZip));
                parametersList.Add(new SqlParameter("@Bill_Phone", modelBilling.BillingPhone));
                parametersList.Add(new SqlParameter("@Bill_Ext", modelBilling.BillingPhoneExt));
                parametersList.Add(new SqlParameter("@Bill_Fax", modelBilling.BillingFax));
                parametersList.Add(new SqlParameter("@Bill_Mobile", modelBilling.BillingMobilePhone));
                parametersList.Add(new SqlParameter("@Bill_EMail", modelBilling.BillingEMail));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);

                result = resultSet.Rows[0][0] == DBNull.Value ? modelBilling.PRJBillingID : Convert.ToInt32(resultSet.Rows[0][0]);
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }

        internal static int saveJobStatus(PRJ01_Headers model)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();
                if (model.PRJID > 0)
                {
                    parametersList.Add(new SqlParameter("@PRJID", model.PRJID));
                    spName = "spATL_PRJ_Upd";
                }
                else
                {
                    spName = "spATL_PRJ_Ins";
                }

                parametersList.Add(new SqlParameter("@PRJ_CommId", model.CommID));
                parametersList.Add(new SqlParameter("@PRJ_ProjectName", model.ProjectName));
                parametersList.Add(new SqlParameter("@PRJ_DivId", model.DivID));
                parametersList.Add(new SqlParameter("@PRJ_MHRateId", model.MhRateID));
                parametersList.Add(new SqlParameter("@PRJ_JobStatusId", model.JobStatusId));
                parametersList.Add(new SqlParameter("@PRJ_DriveTime1Way", model.DriveTime1Way));
                parametersList.Add(new SqlParameter("@JobSiteId", model.JobSiteId));
                parametersList.Add(new SqlParameter("@BillingId", model.PRJBillingID));
                parametersList.Add(new SqlParameter("@PRJ_Notes", model.PRJNotes));
                parametersList.Add(new SqlParameter("@PRJ_IsCommercial", model.IsCommercial));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);

                result = resultSet.Rows[0][0] == DBNull.Value ? model.PRJID : Convert.ToInt32(resultSet.Rows[0][0]);
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }

        internal static SAL01_Company getCompanyAddressDetails(string companyId)
        {
            DataTable result = null;
            result = SqlHelper.ExecuteDataset(_myConnection, "spATL_PRJ_GetComp_Address",
                new SqlParameter("@CompID", companyId)).Tables[0].AsEnumerable().CopyToDataTable();

            SAL01_Company company = new SAL01_Company();
            company.SalCompAddress = Convert.ToString(result.Rows[0]["SalCompAddress"]);
            company.SalCompCity = Convert.ToString(result.Rows[0]["SalCompCity"]);
            company.SalCompState = Convert.ToString(result.Rows[0]["SalCompState"]);
            company.SalCompZip = Convert.ToString(result.Rows[0]["SalCompZip"]);
            company.SalCompPhone = Convert.ToString(result.Rows[0]["SalCompPhone"]);
            company.SalCompPhoneExt = Convert.ToString(result.Rows[0]["salCompPhoneExt"]);
            company.SalCompFax = Convert.ToString(result.Rows[0]["SalCompFax"]);
            company.SalCompMobile = Convert.ToString(result.Rows[0]["SalCompMobile"]);
            company.SalCompEMail = Convert.ToString(result.Rows[0]["SalCompEmail"]);
            // {
            //    //SalCompAddress = r.Field<string>("SalCompAddress")
            //    //SalCompCity = r.Field<string>("SalCompCity"),
            //    //SalCompState = r.Field<string>("SalCompState"),
            //    //SalCompZip = r.Field<string>("SalCompZip"),
            //    //SalCompPhone = r.Field<string>("SalCompPhone"),
            //    //SalCompPhoneExt = r.Field<string>("salCompPhoneExt"),
            //    //SalCompFax = r.Field<string>("SalCompFax"),
            //    //SalCompMobile = r.Field<string>("SalCompMobile"),
            //    //SalCompEMail = r.Field<string>("SalCompEmail"),
            //});
            return company;
        }

        internal static DataSet LoadComboForHeight(string PRJID)
        {
            DataSet resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                "spATL_STYLE_HEIGHT_ddl",
                                new SqlParameter("@PRJID", PRJID));
            return resultSet;
        }

        internal static BID01_Headers SelectHeightByBid(int BIDID)
        {
            BID01_Headers header = new BID01_Headers();
            BID03_MaterialHeader material = new BID03_MaterialHeader();
            try
            {
                var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                    "spATL_LBID_BIDHdr_Get",
                                    new SqlParameter("@BIDID", BIDID)).Tables[0];

                if (resultSet.Rows.Count > 0)
                {
                    header.BIDID = Convert.ToInt32(resultSet.Rows[0]["BIDID"]);
                    header.FenceTypeID = Convert.ToInt32(resultSet.Rows[0]["FenceTypeID"]);
                    header.FenceHtID = Convert.ToInt32(resultSet.Rows[0]["FenceHtID"]);
                    header.FtRangeID = Convert.ToInt32(resultSet.Rows[0]["FtRangeID"]);
                    header.DigTypeID = Convert.ToInt32(resultSet.Rows[0]["DigTypeID"]);
                    header.QtyOfBI = Convert.ToInt32(resultSet.Rows[0]["QtyOfBI"]);
                    header.UnitOfMeasure = Convert.ToString(resultSet.Rows[0]["UnitOfMeasure"]);
                    header.BIType = Convert.ToString(resultSet.Rows[0]["BITypeId"]);
                    header.RelatedBI = Convert.ToString(resultSet.Rows[0]["RelatedBI"]);
                    header.CalcDriveTimeFlag = Convert.ToByte(resultSet.Rows[0]["CalcDriveTimeFlag"]);
                    header.LaborDiscount = Convert.ToDecimal(resultSet.Rows[0]["LaborDiscount"]);
                    header.LbrHoldBack = Convert.ToDecimal(resultSet.Rows[0]["LabrHoldBack"]);
                    header.TaxCalcTypeID = Convert.ToInt32(resultSet.Rows[0]["TaxCalcTypeID"]);
                    header.BIDName = Convert.ToString(resultSet.Rows[0]["BIDName"]);
                    header.BIDMatHeaderID = Convert.ToInt32(resultSet.Rows[0]["BIDMatHeaderID"]);
                    header.CFSFileName = Convert.ToString(resultSet.Rows[0]["CfsFileName"]);
                    header.OverrideCost = Convert.ToBoolean(resultSet.Rows[0]["OverRideCost"]);
                    header.MaterialCost = Convert.ToString(resultSet.Rows[0]["OverRiddenCost"]);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return header;
        }

        internal static List<BidItems> GetBidItemsByProject(int PRJID)
        {
            var lstBidItems = new List<BidItems>();

            try
            {
                var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                    "spATL_VIEWBID",
                                    new SqlParameter("@PRJID", PRJID)).Tables[0];

                foreach (DataRow item in resultSet.Rows)
                {
                    var bidItem = new BidItems();
                    bidItem.PRJID = Convert.ToString(item["PRJID"]);
                    bidItem.BidItemId = Convert.ToString(item["BIDID"]);
                    bidItem.PreBid = Convert.ToString(item["RelatedBI"]);
                    bidItem.BidItemName = Convert.ToString(item["BIDName"]);
                    bidItem.FenceTypeId = Convert.ToString(item["FenceTypeID"]);
                    bidItem.FenceType = Convert.ToString(item["FenceType"]);
                    if (item["DateActivated"] != DBNull.Value)
                    {
                        bidItem.DateActivated = Convert.ToDateTime(item["DateActivated"]).ToShortDateString();
                    }
                    bidItem.PreTxSoldFor = Convert.ToString(item["PreTxSoldFor"]); 
                    lstBidItems.Add(bidItem);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return lstBidItems;
        }

        internal static int saveStyleHeight(BID01_Headers header)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();

                spName = "spATL_LBID_BIDHdr_Ins";
                if (header.BIDID > 0)
                {
                    parametersList.Add(new SqlParameter("@BIDID", header.BIDID));
                }
                parametersList.Add(new SqlParameter("@PRJID", header.PRJID));
                parametersList.Add(new SqlParameter("@BIDName", header.BIDName));
                parametersList.Add(new SqlParameter("@DateActivated", DateTime.Now));
                parametersList.Add(new SqlParameter("@DateEstStart", DateTime.UtcNow));
                parametersList.Add(new SqlParameter("@BIDStatusID", header.BIDStatusID));
                parametersList.Add(new SqlParameter("@InRollup", header.InRollup));
                parametersList.Add(new SqlParameter("@CalcDriveTimeFlag", header.CalcDriveTimeFlag));
                parametersList.Add(new SqlParameter("@LaborDiscount", header.LaborDiscount));
                parametersList.Add(new SqlParameter("@LbrHoldBack", header.LbrHoldBack));
                parametersList.Add(new SqlParameter("@TaxCalcTypeID", header.TaxCalcTypeID));
                parametersList.Add(new SqlParameter("@SalTxPer", header.SalTxPer));
                parametersList.Add(new SqlParameter("@EditBidItemFlag", header.EditBidItemFlag));
                parametersList.Add(new SqlParameter("@FenceTypeID", header.FenceTypeID));
                parametersList.Add(new SqlParameter("@FenceHtID", header.FenceHtID));
                parametersList.Add(new SqlParameter("@PercentOfHtStd", header.PercentOfHtStd));
                parametersList.Add(new SqlParameter("@FtRangeID", header.FtRangeID));
                parametersList.Add(new SqlParameter("@PercentOfFtRangeStd", header.PercentOfFtRangeStd));
                parametersList.Add(new SqlParameter("@DigTypeID", header.DigTypeID));
                parametersList.Add(new SqlParameter("@PercentOfDigStandard", header.PercentOfDigStandard));
                parametersList.Add(new SqlParameter("@SupervisonMarkup", header.SupervisonMarkup));
                parametersList.Add(new SqlParameter("@MaterialMarkUp", header.MaterialMarkUp));
                parametersList.Add(new SqlParameter("@LaborMarkUp", header.LaborMarkUp));
                parametersList.Add(new SqlParameter("@JobMarkUp", header.JobMarkUp));
                parametersList.Add(new SqlParameter("@PFPJobMarkup", header.PFPJobMarkup));
                parametersList.Add(new SqlParameter("@QtyOfBI", header.QtyOfBI));
                parametersList.Add(new SqlParameter("@UnitOfMeasure", header.UnitOfMeasure));
                parametersList.Add(new SqlParameter("@BIType", header.BIType));
                parametersList.Add(new SqlParameter("@RelatedBI", header.RelatedBI));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);
                result = resultSet.Rows[0][0] == DBNull.Value ? header.BIDID : Convert.ToInt32(resultSet.Rows[0][0]);
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }

        internal static DataSet getLabourDetailsByBid(string bid)
        {
            var BidLabour = new DataSet();
            try
            {
                BidLabour = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                    "spATL_LBID_Labor_Get",
                                    new SqlParameter("@BIDID", bid));
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return BidLabour;
        }

        internal static int saveMaterialHeader(BID03_MaterialHeader matHeader)
        {
            int result = 0;
            try
            {
                string spName = string.Empty;
                List<SqlParameter> parametersList = new List<SqlParameter>();

                spName = "spATL_LBID_MatHdr_INS";

                if (matHeader.BIDMatHeaderID > 0)
                {
                    parametersList.Add(new SqlParameter("@BIDMatHeaderID", matHeader.BIDMatHeaderID));
                }
                parametersList.Add(new SqlParameter("@BIDID", matHeader.BIDID));
                parametersList.Add(new SqlParameter("@ImportedFlag", matHeader.ImportedFlag));
                parametersList.Add(new SqlParameter("@CfsFileName", matHeader.CfsFileName));
                parametersList.Add(new SqlParameter("@EmployeeID", matHeader.EmployeeID));
                parametersList.Add(new SqlParameter("@OverRideCost", matHeader.OverRideCost));
                parametersList.Add(new SqlParameter("@OverRiddenCost", matHeader.OverRiddenCost));

                DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 spName, parametersList.ToArray()).Tables[0]);

                if ((resultSet.Rows[0][0]) == DBNull.Value)
                {
                    result = matHeader.BIDMatHeaderID;
                }
                else
                {
                    result = Convert.ToInt32(resultSet.Rows[0][0]);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
                result = -1;
            }
            return (result);
        }

        internal static List<LabourDdls> getLabourDetails(string fenceType, int fieldLbrTypeID, string bidid)
        {
            var labourddls = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 "spATL_LBID_EnterLbrddl",
                                  new SqlParameter("@BIDID", bidid),
                                 new SqlParameter("@FieldLbrTypeID", fieldLbrTypeID),
                                 new SqlParameter("@FenceTypeId", fenceType)).Tables[0].AsEnumerable());

            List<LabourDdls> ListlabourDDls = new List<LabourDdls>();

            foreach (var item in labourddls)
            {
                ListlabourDDls.Add(new LabourDdls
                {
                    FieldLbrDtlsID = Convert.ToString(item["FieldLbrDtlsID"]),
                    FieldLbrDesc = Convert.ToString(item["FieldLbrDesc"]),
                    FieldLbrUom = Convert.ToString(item["FieldLbrUom"]),
                    FieldLbrMhs = Convert.ToString(item["FieldLbrMhs"]),
                    FenceTypeID = Convert.ToString(item["FenceTypeID"]),
                    FenceType = Convert.ToString(item["FenceType"]),
                    FenceFamilyId = Convert.ToString(item["FenceFamilyId"]),
                    Description = Convert.ToString(item["Description"]),
                    FieldLbrTypeID = Convert.ToString(item["FieldLbrTypeID"]),
                    FieldLbrTypeDesc = Convert.ToString(item["FieldLbrTypeDesc"])
                });
            }

            return ListlabourDDls;
        }

        internal static List<LabourLabels> getLabourLabels(string fenceType)
        {
            var labourLabels = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                 "spATL_LBID_EnterLbrLabel",
                                 new SqlParameter("@FenceTypeId", fenceType)).Tables[0].AsEnumerable());
            List<LabourLabels> listLabour = new List<LabourLabels>();
            foreach (var item in labourLabels)
            {
                LabourLabels lables = new LabourLabels
                {
                    FieldLbrTypeID = Convert.ToInt32(item["FieldLbrTypeID"]),
                    ElemLabelText = Convert.ToString(item["ElemLabelText"]),
                    Sort = Convert.ToInt32(item["Sort"]),
                    FenceFamilyID = Convert.ToInt32(item["FenceFamilyID"]),
                    FenceTypeID = Convert.ToInt32(item["FenceTypeID"])
                };
                listLabour.Add(lables);
            }
            return listLabour;
        }

        internal static DataSet getComboForLabourSections(string bid)
        {
            DataSet resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                "spddl_LBID_Labor", new SqlParameter("@BIDID", bid));
            return resultSet;
        }

        internal static List<LabourDetails> saveMainLabour(List<LabourDetails> labourDetails)
        {
            int i = 0;
            if (labourDetails != null)
            {
                foreach (var item in labourDetails)
                {
                    try
                    {
                        string spName = string.Empty;
                        List<SqlParameter> parametersList = new List<SqlParameter>();
                        spName = "spATL_LBID_MainLabor_Ins";
                        if (item.BIDLaborItemsID > 0)
                        {
                            parametersList.Add(new SqlParameter("@BIDLaborItemsID", item.BIDLaborItemsID));
                        }
                        parametersList.Add(new SqlParameter("@BIDId", item.BIDID));
                        parametersList.Add(new SqlParameter("@FieldLbrDtlsID", item.FieldLaborId));
                        parametersList.Add(new SqlParameter("@Qty", item.Quantity));

                        DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                         spName, parametersList.ToArray()).Tables[0]);
                        labourDetails[i].BIDLaborItemsID = Convert.ToInt32(resultSet.Rows[0][0]);
                        labourDetails[i].RowNum = item.RowNum;
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveErr(ex);
                        labourDetails = null;
                    }
                }
            }
            return labourDetails;
        }

        internal static List<BID07_Concrete> saveConcrete(List<BID07_Concrete> concreteDetails)
        {
            int i = 0;
            if (concreteDetails != null)
            {
                foreach (var item in concreteDetails)
                {
                    try
                    {
                        string spName = string.Empty;
                        List<SqlParameter> parametersList = new List<SqlParameter>();
                        spName = "spATL_LBID_Concrete_Ins";
                        if (item.CONCID > 0)
                        {
                            parametersList.Add(new SqlParameter("@CONCID", item.CONCID));
                        }
                        parametersList.Add(new SqlParameter("@BIDId", item.BIDID));
                        parametersList.Add(new SqlParameter("@PstTypId", item.PstTypId));
                        parametersList.Add(new SqlParameter("@ConcreteType", item.ConcreteType));
                        parametersList.Add(new SqlParameter("@HoleQty", item.HoleQty));
                        parametersList.Add(new SqlParameter("@HoleDia", item.HoleDia));
                        parametersList.Add(new SqlParameter("@HoleDepth", item.HoleDepth));
                        parametersList.Add(new SqlParameter("@RoundFactor", item.RoundFactor));

                        DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                         spName, parametersList.ToArray()).Tables[0]);
                        concreteDetails[i].CONCID = Convert.ToInt32(resultSet.Rows[0][0]);
                        concreteDetails[i].RowNum = item.RowNum;
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveErr(ex);
                        concreteDetails = null;
                    }
                }
            }
            return concreteDetails;
        }

        internal static List<BID08_OtherCosts> saveOtherCostsDetails(List<BID08_OtherCosts> otherCostsDetails)
        {
            int i = 0;
            if (otherCostsDetails != null)
            {
                foreach (var item in otherCostsDetails)
                {
                    try
                    {
                        string spName = string.Empty;
                        List<SqlParameter> parametersList = new List<SqlParameter>();
                        spName = "spATL_LBID_OtherCosts_Ins";
                        if (item.OtherCostID > 0)
                        {
                            parametersList.Add(new SqlParameter("@OtherCostID", item.OtherCostID));
                        }
                        parametersList.Add(new SqlParameter("@BIDId", item.BIDID));
                        parametersList.Add(new SqlParameter("@OtherCostTypeID", item.OtherCostTypeID));
                        parametersList.Add(new SqlParameter("@Qty", item.Qty));
                        parametersList.Add(new SqlParameter("@Cost", item.Cost));

                        DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                         spName, parametersList.ToArray()).Tables[0]);
                        otherCostsDetails[i].OtherCostID = Convert.ToInt32(resultSet.Rows[0][0]);
                        otherCostsDetails[i].RowNum = item.RowNum;
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveErr(ex);
                        otherCostsDetails = null;
                    }
                }
            }
            return otherCostsDetails;
        }

        internal static int RemoveLabourSectionRow(int id, string section, string idName)
        {
            string tableName = "";

            switch (section)
            {
                case "tblConcrete":
                    tableName = "BID07_Concrete";
                    break;

                case "tblOtherCost":
                    tableName = "BID08_OtherCosts";
                    break;

                case "tblCrewProfile":
                    tableName = "BID06_CrewProfile";
                    break;

                case "tblEquipment":
                    tableName = "BID09_EquipmentBurden";
                    break;
            }
            idName = idName.Split('_')[1];
            string query = "Delete from " + tableName + " where " + idName + "=" + id;
            var resultSet = Convert.ToInt32(SqlHelper.ExecuteNonQuery(_myConnection, CommandType.Text,
                                       query));

            return resultSet;
        }

        internal static string getUOMDesc(string id)
        {
            string query = "select top 1 FieldLbrUom from vLaborDetail where FieldLbrDtlsID =" + id;
            return Convert.ToString(SqlHelper.ExecuteScalar(_myConnection, CommandType.Text,
                                       query));
        }

        internal static List<BID06_CrewProfile> saveCrewProfile(List<BID06_CrewProfile> crewProfile)
        {
            int i = 0;
            if (crewProfile != null)
            {
                foreach (var item in crewProfile)
                {
                    try
                    {
                        string spName = string.Empty;
                        List<SqlParameter> parametersList = new List<SqlParameter>();
                        spName = "spATL_LBID_CrewSize_Ins";
                        if (item.BIDDefineCrewID > 0)
                        {
                            parametersList.Add(new SqlParameter("@BIDDefineCrewID", item.BIDDefineCrewID));
                        }
                        parametersList.Add(new SqlParameter("@BIDID", item.BIDID));
                        parametersList.Add(new SqlParameter("@CrewPositionID", item.CrewPositionID));
                        parametersList.Add(new SqlParameter("@HeadCount", item.HeadCount));

                        DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                         spName, parametersList.ToArray()).Tables[0]);
                        crewProfile[i].BIDDefineCrewID = Convert.ToInt32(resultSet.Rows[0][0]);
                        crewProfile[i].RowNum = item.RowNum;
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveErr(ex);
                        crewProfile = null;
                    }
                }
            }
            return crewProfile;
        }

        internal static List<BID09_EquipmentBurden> saveEquipmentBurden(List<BID09_EquipmentBurden> equipmentBurden)
        {
            int i = 0;
            if (equipmentBurden != null)
            {
                foreach (var item in equipmentBurden)
                {
                    try
                    {
                        string spName = string.Empty;
                        List<SqlParameter> parametersList = new List<SqlParameter>();
                        spName = "spATL_LBID_Equipment_Ins";
                        if (item.EquipBurdID > 0)
                        {
                            parametersList.Add(new SqlParameter("@EquipBurdID", item.EquipBurdID));
                        }
                        parametersList.Add(new SqlParameter("@BIDId", item.BIDID));
                        parametersList.Add(new SqlParameter("@EquipCostID", item.EquipCostID));
                        parametersList.Add(new SqlParameter("@Qty", item.Qty));

                        DataTable resultSet = (SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                         spName, parametersList.ToArray()).Tables[0]);
                        equipmentBurden[i].EquipBurdID = Convert.ToInt32(resultSet.Rows[0][0]);
                        equipmentBurden[i].RowNum = item.RowNum;
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveErr(ex);
                        equipmentBurden = null;
                    }
                }
            }
            return equipmentBurden;
        }

        internal static int saveJobActivation(JobActivationChecklistModel model)
        {
            int result = 0;
            try
            {
                List<SqlParameter> parametersList = new List<SqlParameter>();
                parametersList.Add(new SqlParameter("@PRJID", model.PRJID));
                parametersList.Add(new SqlParameter("@CustomerTypeID", model.projectInformation.CustomerType));
                parametersList.Add(new SqlParameter("@JobTypeId", model.projectInformation.Jobtype));
                parametersList.Add(new SqlParameter("@CustomerBid_JobNum", model.projectInformation.CustomerBidReference));
                parametersList.Add(new SqlParameter("@Scope", model.projectInformation.ScopeWorkToBePerformed));
                parametersList.Add(new SqlParameter("@Contract", model.contractPaperWork.CopyOfContractorPO));
                parametersList.Add(new SqlParameter("@ContractComments", model.contractPaperWork.CopyOfContractorPOComments));
                parametersList.Add(new SqlParameter("@ScopePhased", model.contractPaperWork.BrokenScopephases));
                parametersList.Add(new SqlParameter("@ScopePhasedComments", model.contractPaperWork.BrokenScopephasesComments));
                parametersList.Add(new SqlParameter("@BIRollups", model.contractPaperWork.BidRollUp));
                parametersList.Add(new SqlParameter("@BIRollupsComments", model.contractPaperWork.BidRollUpComments));
                parametersList.Add(new SqlParameter("@Packslip", model.contractPaperWork.PackCFSPIRollUp));
                parametersList.Add(new SqlParameter("@PackslipComments", model.contractPaperWork.PackCFSPIRollUpcomments));
                parametersList.Add(new SqlParameter("@Quotes", model.contractPaperWork.ApplicableQuote));
                parametersList.Add(new SqlParameter("@QuotesComments", model.contractPaperWork.ApplicableQuoteComments));
                parametersList.Add(new SqlParameter("@Drawings", model.contractPaperWork.DrawingConditions));
                parametersList.Add(new SqlParameter("@DrawingsComments", model.contractPaperWork.DrawingConditionsComments));
                parametersList.Add(new SqlParameter("@Photos", model.contractPaperWork.SitePhotos));
                parametersList.Add(new SqlParameter("@PhotoComments", model.contractPaperWork.SitePhotosComments));
                parametersList.Add(new SqlParameter("@HardCard", model.contractPaperWork.HardCard));
                parametersList.Add(new SqlParameter("@HardCardComments", model.contractPaperWork.HardCardComments));
                parametersList.Add(new SqlParameter("@PayEnvelope", model.contractPaperWork.PayEnvelope));
                parametersList.Add(new SqlParameter("@PayEnvelopeComments", model.contractPaperWork.PayEnvelopeComments));
                parametersList.Add(new SqlParameter("@Bonding", model.bondingInsurance.InsuranceCertification));
                parametersList.Add(new SqlParameter("@BondingComments", model.bondingInsurance.InsuranceCertificationComments));
                parametersList.Add(new SqlParameter("@SafetyOfficer", model.safetyRequirements.SafetyOfficer));
                parametersList.Add(new SqlParameter("@SafetyOfficerComments", model.safetyRequirements.SafetyOfficerComments));
                parametersList.Add(new SqlParameter("@PreStartSafetyMeeting", model.safetyRequirements.SafetyMeeting));
                parametersList.Add(new SqlParameter("@PreStartSafetyMeetingComments", model.safetyRequirements.SafetyMeetingComments));
                parametersList.Add(new SqlParameter("@DailySafetyMeetings", model.safetyRequirements.DailySafetyMeeting));
                parametersList.Add(new SqlParameter("@DailySafetyMeetingComments", model.safetyRequirements.DailySafetyMeetingComments));
                parametersList.Add(new SqlParameter("@PPE", model.safetyRequirements.PPENeeded));
                parametersList.Add(new SqlParameter("@PPEComments", model.safetyRequirements.PPENeededComments));
                parametersList.Add(new SqlParameter("@Fall", model.safetyRequirements.FallProtection));
                parametersList.Add(new SqlParameter("@FallComments", model.safetyRequirements.FallProtectionComments));
                parametersList.Add(new SqlParameter("@EquipCerts", model.safetyRequirements.EquipmentCertification));
                parametersList.Add(new SqlParameter("@EquipCertComments", model.safetyRequirements.EquipmentCertificationComments));
                parametersList.Add(new SqlParameter("@OtherHaz", model.safetyRequirements.OtherHazards));
                parametersList.Add(new SqlParameter("@OtherHazComments", model.safetyRequirements.OtherHazardsComments));
                parametersList.Add(new SqlParameter("@OtherComments", model.otherImportantFactors.OtherPertinentInformation));
                parametersList.Add(new SqlParameter("@DateCompleted", model.DateCompleted));

                var id = SqlHelper.ExecuteNonQuery(_myConnection, CommandType.StoredProcedure, "spATL_PRJ_JobActChkLst_InsUpd",
                                      parametersList.ToArray());
                result = (Convert.ToInt32(id) == -1) ? 1 : result;
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return result;
        }

        public static JobActivationChecklistModel JobActivationLookup(JobActivationChecklistModel objActivation)
        {
            try
            {
                var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                     "spATL_PRJ_JobActChkLst_LkUp",
                                     new SqlParameter("@PRJID", objActivation.PRJID));

                objActivation.ListCustomersType = new List<Setup17_CustomerType>();
                foreach (DataRow item in resultSet.Tables[0].Rows)
                {
                    var customer = new Setup17_CustomerType();
                    customer.CustomerTypeId = Convert.ToInt32(item["CustomerTypeId"]);
                    customer.Description = Convert.ToString(item["Description"]);
                    objActivation.ListCustomersType.Add(customer);
                }

                objActivation.ListJobTypes = new List<Setup18_JobTypes>();
                foreach (DataRow item in resultSet.Tables[1].Rows)
                {
                    var jobType = new Setup18_JobTypes();
                    jobType.JobTypeId = Convert.ToInt32(item["JobTypeId"]);
                    jobType.JobTypeDesc = Convert.ToString(item["JobTypeDesc"]);
                    objActivation.ListJobTypes.Add(jobType);
                }

                objActivation.ListResponses = new List<Setup19_Responses>();
                foreach (DataRow item in resultSet.Tables[2].Rows)
                {
                    var response = new Setup19_Responses();
                    response.ResponseId = Convert.ToInt32(item["ResponseId"]);
                    response.Response = Convert.ToString(item["Response"]);
                    objActivation.ListResponses.Add(response);
                }

                foreach (DataRow item in resultSet.Tables[3].Rows)
                {
                    objActivation.PRJID = Convert.ToInt32(item["PRJID"]);
                    objActivation.projectInformation.JobNumber = String.Format("{0}{1}", Convert.ToString(item["CommID"]), Convert.ToInt32(item["PRJID"]));
                    objActivation.projectInformation.CompanyName = Convert.ToString(item["AtlasCompanyName"]);
                    objActivation.projectInformation.CustomerProfile = new Profile();
                    objActivation.projectInformation.CustomerProfile.Name = Convert.ToString(item["CustomerContractorName"]);
                    objActivation.projectInformation.CustomerProfile.Address = Convert.ToString(item["CustomerContractorAddress"]);
                    objActivation.projectInformation.CustomerProfile.City = Convert.ToString(item["CustomerContractorCity"]);
                    objActivation.projectInformation.CustomerProfile.State = Convert.ToString(item["CustomerContractorState"]);
                    objActivation.projectInformation.CustomerProfile.Zip = Convert.ToString(item["CustomerContractorZip"]);
                    objActivation.projectInformation.CustomerProfile.Zip = Convert.ToString(item["CustomerContractorPhone"]);
                    objActivation.projectInformation.CustomerProfile.Zip = Convert.ToString(item["CustomerContractorPhoneExt"]);

                    objActivation.projectInformation.ProjectProfile = new Profile();
                    objActivation.projectInformation.ProjectProfile.Name = Convert.ToString(item["ProjectName"]);
                    objActivation.projectInformation.ProjectProfile.Address = Convert.ToString(item["ProjectAddress"]);
                    objActivation.projectInformation.ProjectProfile.City = Convert.ToString(item["ProjectCity"]);
                    objActivation.projectInformation.ProjectProfile.State = Convert.ToString(item["ProjectState"]);
                    objActivation.projectInformation.ProjectProfile.Zip = Convert.ToString(item["ProjectZip"]);
                    objActivation.projectInformation.ProjectProfile.PhoneNumber = Convert.ToString(item["ProjectPhone"]);
                    objActivation.projectInformation.ProjectProfile.Extension = Convert.ToString(item["ProjectPhoneExt"]);
                    objActivation.projectInformation.ProjectProfile.ContactName = Convert.ToString(item["ContactName"]);
                    objActivation.projectInformation.TypeOfLabour = Convert.ToString(item["TypeOfLabor"]);
                    objActivation.projectInformation.Estimator = Convert.ToString(item["Estimator"]);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return objActivation;
        }

        internal static JobActivationChecklistModel getProjectActivationDetails(JobActivationChecklistModel objActivation)
        {
            try
            {
                var resultSet = SqlHelper.ExecuteDataset(_myConnection, CommandType.StoredProcedure,
                                    "spATL_PRJ_JobActChkLst_GetDtls",
                                    new SqlParameter("@PRJID", objActivation.PRJID));
                objActivation.projectInformation = new ProjectInformation();
                objActivation.contractPaperWork = new ContractPaperWork();
                objActivation.bondingInsurance = new BondingInsurance();
                objActivation.safetyRequirements = new SafetyRequirements();
                objActivation.otherImportantFactors = new OtherImportantFactors();
                foreach (DataRow item in resultSet.Tables[0].Rows)
                {
                    objActivation.projectInformation.CustomerType = Convert.ToString(item["CustomerTypeID"]);
                    objActivation.projectInformation.Jobtype = Convert.ToString(item["JobTypeId"]);

                    objActivation.projectInformation.CustomerBidReference = Convert.ToString(item["CustomerBid_JobNum"]);

                    objActivation.projectInformation.ScopeWorkToBePerformed = Convert.ToString(item["Scope"]);

                    objActivation.contractPaperWork.CopyOfContractorPO = Convert.ToString(item["Contract"]);
                    objActivation.contractPaperWork.CopyOfContractorPOComments = Convert.ToString(item["ContractComments"]);

                    objActivation.contractPaperWork.BrokenScopephases = Convert.ToString(item["ScopePhased"]);
                    objActivation.contractPaperWork.BrokenScopephasesComments = Convert.ToString(item["ScopePhasedComments"]);

                    objActivation.contractPaperWork.BidRollUp = Convert.ToString(item["BIRollups"]);
                    objActivation.contractPaperWork.BidRollUpComments = Convert.ToString(item["BIRollupsComments"]);

                    objActivation.contractPaperWork.PackCFSPIRollUp = Convert.ToString(item["Packslip"]);
                    objActivation.contractPaperWork.PackCFSPIRollUpcomments = Convert.ToString(item["PackslipComments"]);

                    objActivation.contractPaperWork.ApplicableQuote = Convert.ToString(item["Quotes"]);
                    objActivation.contractPaperWork.ApplicableQuoteComments = Convert.ToString(item["QuotesComments"]);

                    objActivation.contractPaperWork.SitePhotos = Convert.ToString(item["Photos"]);
                    objActivation.contractPaperWork.SitePhotosComments = Convert.ToString(item["PhotoComments"]);

                    objActivation.contractPaperWork.DrawingConditions = Convert.ToString(item["Drawings"]);
                    objActivation.contractPaperWork.DrawingConditionsComments = Convert.ToString(item["DrawingsComments"]);

                    objActivation.contractPaperWork.HardCard = Convert.ToString(item["HardCard"]);
                    objActivation.contractPaperWork.HardCardComments = Convert.ToString(item["HardCardComments"]);

                    objActivation.contractPaperWork.PayEnvelope = Convert.ToString(item["PayEnvelope"]);
                    objActivation.contractPaperWork.PayEnvelopeComments = Convert.ToString(item["PayEnvelopeComments"]);

                    /************/
                    objActivation.bondingInsurance.InsuranceCertification = Convert.ToString(item["Bonding"]);
                    objActivation.bondingInsurance.InsuranceCertificationComments = Convert.ToString(item["BondingComments"]);
                    /************/

                    objActivation.safetyRequirements.SafetyOfficer = Convert.ToString(item["SafetyOfficer"]);
                    objActivation.safetyRequirements.SafetyOfficerComments = Convert.ToString(item["SafetyOfficerComments"]);

                    objActivation.safetyRequirements.SafetyMeeting = Convert.ToString(item["PreStartSafetyMeeting"]);
                    objActivation.safetyRequirements.SafetyMeetingComments = Convert.ToString(item["PreStartSafetyMeetingComments"]);

                    objActivation.safetyRequirements.DailySafetyMeeting = Convert.ToString(item["DailySafetyMeetings"]);
                    objActivation.safetyRequirements.DailySafetyMeetingComments = Convert.ToString(item["DailySafetyMeetingComments"]);

                    objActivation.safetyRequirements.PPENeeded = Convert.ToString(item["PPE"]);
                    objActivation.safetyRequirements.PPENeededComments = Convert.ToString(item["PPEComments"]);

                    objActivation.safetyRequirements.FallProtection = Convert.ToString(item["Fall"]);
                    objActivation.safetyRequirements.FallProtectionComments = Convert.ToString(item["FallComments"]);

                    objActivation.safetyRequirements.EquipmentCertification = Convert.ToString(item["EquipCerts"]);
                    objActivation.safetyRequirements.EquipmentCertificationComments = Convert.ToString(item["EquipCertComments"]);

                    objActivation.safetyRequirements.OtherHazards = Convert.ToString(item["OtherHaz"]);
                    objActivation.safetyRequirements.OtherHazardsComments = Convert.ToString(item["OtherHazComments"]);

                    objActivation.otherImportantFactors.OtherPertinentInformation = Convert.ToString(item["OtherComments"]);

                    objActivation.DateCompleted = Convert.ToDateTime(item["DateCompleted"]).ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
            return objActivation;
        }
    }
}