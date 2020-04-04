using Atlas.DAL;
using Atlas.DataAccess.Entity;
using Atlas.Models;
using Atlas.Models.DBO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Atlas.Controllers
{
    [Authorize]
    public partial class ProjectController : Controller
    {
        [Route("Project")]
        [Route("Project/index")]
        [Route("Project/index/id/{id}")]
        public ActionResult Index(string id = "0")
        {
            LoadProjectCombos();
            if (!string.IsNullOrWhiteSpace(id) && Convert.ToInt32(id) >0)
            {
                Session["PRJID"] = id;
            }

            if (Session["PRJID"] != null)
            {
                id = (string)Session["PRJID"];
            }

            ProjectViewModel projectvm = new ProjectViewModel();
            if (Convert.ToInt32(id) > 0)
            {
                int PrjId = Convert.ToInt32(id);
                projectvm = ProjectDAL.getProjectFormDetails(PrjId);
            }
            return View(projectvm);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        private void LoadProjectCombos()
        {
            ViewBag.CompanyList = BindCompanies();
            ViewBag.StatesList = new SelectList(DataAccess.Entity.Common.getStatesList(), "State", "StateName");
            DataSet resultSet = ProjectDAL.getRateTypes((string)Session["CommID"]);

            List<Setup02_MhRates> lstRates = new List<Setup02_MhRates>();
            foreach (var record in resultSet.Tables[1].AsEnumerable())
            {
                Setup02_MhRates source = new Setup02_MhRates();

                source.MhRateID = Convert.ToString(record["MhRateID"]);
                source.MhRate = Convert.ToString(record["MhRate"]);
                lstRates.Add(source);
            }

            ViewBag.RateTypesList = new SelectList(lstRates, "MhRateID", "MhRate");

            List<PRJ05_JobStatus> lstJobStatus = new List<PRJ05_JobStatus>();
            foreach (var record in resultSet.Tables[2].AsEnumerable())
            {
                PRJ05_JobStatus source = new PRJ05_JobStatus();

                source.JobStatusId = Convert.ToString(record["JobStatusId"]);
                source.JobStatusName = Convert.ToString(record["JobStatusName"]);
                lstJobStatus.Add(source);
            }
            ViewBag.JobStatusList = new SelectList(lstJobStatus, "JobStatusId", "JobStatusName");

            List<Setup01_Divisions> lstDivisions = new List<Setup01_Divisions>();
            foreach (var record in resultSet.Tables[4].AsEnumerable())
            {
                Setup01_Divisions source = new Setup01_Divisions();

                source.DivID = Convert.ToString(record["DivID"]);
                source.Division = Convert.ToString(record["Division"]);
                lstDivisions.Add(source);
            }
            ViewBag.DivisionList = new SelectList(lstDivisions, "DivID", "DivID");
        }

        [HttpPost]
        public ActionResult Create(ProjectViewModel modelJobBilling)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    modelJobBilling.JobSites.JobSitePhone = DataAccess.Entity.Common.FormatPhoneText(modelJobBilling.JobSites.JobSitePhone);
                    modelJobBilling.JobSites.JobSiteMobilePhone = DataAccess.Entity.Common.FormatPhoneText(modelJobBilling.JobSites.JobSiteMobilePhone);
                    modelJobBilling.JobSites.JobSiteFax = DataAccess.Entity.Common.FormatPhoneText(modelJobBilling.JobSites.JobSiteFax);
                    var result = ProjectDAL.saveJobSiteInformation(modelJobBilling.JobSites);

                    if (result <= 0)
                    {
                        ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                        LoadProjectCombos();
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, BusinessConstants.duplicateRecord);
                    LoadProjectCombos();
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                LoadProjectCombos();
                ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                return View(modelJobBilling);
            }
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

        [HttpPost]
        public ActionResult getCompanyAddressDetails(string companyId)
        {
            SAL01_Company company = new SAL01_Company();
            if (!string.IsNullOrWhiteSpace(companyId))
            {
                company = ProjectDAL.getCompanyAddressDetails(companyId);
            }
            return Json(company);
        }

        [HttpPost]
        public ActionResult saveJobSitesInfo(PRJ04_JobSites JobSites)
        {
            int result = -2;
            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["CommID"])))
            {
                JobSites.JobSitePhone = DataAccess.Entity.Common.FormatPhoneText(JobSites.JobSitePhone);
                JobSites.JobSiteMobilePhone = DataAccess.Entity.Common.FormatPhoneText(JobSites.JobSiteMobilePhone);
                JobSites.JobSiteFax = DataAccess.Entity.Common.FormatPhoneText(JobSites.JobSiteFax);
                result = ProjectDAL.saveJobSiteInformation(JobSites);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult saveBillingInfo(PRJ08_BillingInfo billingInfo)
        {
            int billingID = BusinessConstants.DefaultBillingId;
            
            if (DataAccess.Entity.Common.CheckModelIsEmpty(billingInfo))
            {
                billingInfo.BillingPhone = DataAccess.Entity.Common.FormatPhoneText(billingInfo.BillingPhone);
                billingInfo.BillingMobilePhone = DataAccess.Entity.Common.FormatPhoneText(billingInfo.BillingMobilePhone);
                billingInfo.BillingFax = DataAccess.Entity.Common.FormatPhoneText(billingInfo.BillingFax);
                billingID = ProjectDAL.saveBillingInformation(billingInfo);
            }

            return Json(billingID);
        }

        [HttpPost]
        public ActionResult saveJobSiteDetails(PRJ01_Headers JobDetails)
        {
            JobDetails.CommID = (string)Session["CommID"];
            var result = ProjectDAL.saveJobStatus(JobDetails);
            if (result > 0)
            {
                Session["PRJID"] = result;
                Session["JobStatusId"] = JobDetails.JobStatusId;
            }
            return Json(result);
        }

        public ActionResult material()
        {
            return View();
        }

        [Route("project/selectheight")]
        [Route("Project/selectheight/bid/{bid}")]
        public ActionResult selectheight(string bid = "0")
        {
            if (!string.IsNullOrWhiteSpace((string)Session["CommID"]) && Session["PRJID"] != null && Convert.ToInt32(Session["PRJID"]) > 0)
            {
                LoadComboForHeight();
                if (!string.IsNullOrWhiteSpace(bid) && bid != "0")
                {
                    Session["BidId"] = bid;
                }
                else
                {
                    bid = Convert.ToString(Session["BidId"]);

                }
                if (Session["BidId"] == null || bid != Convert.ToString(Session["BidId"]))
                {
                    Session["BidId"] = bid;
                }

                int BidId = 0;
                BID01_Headers model = new BID01_Headers();
                if (!string.IsNullOrWhiteSpace(bid))
                {
                    BidId = Convert.ToInt32(bid);
                    model = ProjectDAL.SelectHeightByBid(BidId);
                    Session["BIDMATID"] = model.BIDMatHeaderID;
                }

                return View("detail/selectheight", model);
            }
            else
            {
                return Redirect("index");
            }

        }

        public ActionResult viewmakeitems(string BIDMATID)
        {
            if (!string.IsNullOrWhiteSpace(BIDMATID))
            {
                Session["BIDMATID"] = BIDMATID;
            }
            else
            {
                BIDMATID = Convert.ToString(Session["BIDMATID"]);

            }
            if (Session["BIDMATID"] == null || BIDMATID != Convert.ToString(Session["BIDMATID"]))
            {
                Session["BIDMATID"] = BIDMATID;
            }
            List<BID04_MaterialDetail> materialDl = CFSProcessingDAL.GetBidMaterialDetails(Convert.ToInt32(Session["BIDMATID"]));
            return View("detail/viewmakeitems", materialDl);
        }

        private void LoadComboForHeight()
        {
            DataSet ds = ProjectDAL.LoadComboForHeight(Convert.ToString(Session["PRJID"]));
            List<Setup25_FenceTypes> lstFenceTypes = new List<Setup25_FenceTypes>();
            foreach (var record in ds.Tables[0].AsEnumerable())
            {
                Setup25_FenceTypes source = new Setup25_FenceTypes();
                source.FenceTypeID = Convert.ToInt32(record["FenceTypeID"]);
                source.FenceType = Convert.ToString(record["FenceType"]);
                lstFenceTypes.Add(source);
            }
            ViewBag.FenceTypes = new SelectList(lstFenceTypes, "FenceTypeID", "FenceType");

            List<Setup26_FenceHts> lstFenceHts = new List<Setup26_FenceHts>();
            foreach (var record in ds.Tables[1].AsEnumerable())
            {
                Setup26_FenceHts source = new Setup26_FenceHts();
                source.FenceHtID = Convert.ToInt32(record["FenceHtID"]);
                source.FenceHtFt = Convert.ToDecimal(record["FenceHtFt"]);
                source.FenceHtIn = Convert.ToInt32(record["FenceHtIn"]);
                lstFenceHts.Add(source);
            }
            ViewBag.FenceHts = new SelectList(lstFenceHts, "FenceHtID", "FenceHtFt");

            List<Setup27_FootageRanges> lstFootageRanges = new List<Setup27_FootageRanges>();
            foreach (var record in ds.Tables[2].AsEnumerable())
            {
                Setup27_FootageRanges source = new Setup27_FootageRanges();
                source.FtRangeID = Convert.ToInt32(record["FtRangeID"]);
                source.FtRange = Convert.ToString(record["FtRange"]);
                lstFootageRanges.Add(source);
            }
            ViewBag.FootageRanges = new SelectList(lstFootageRanges, "FtRangeID", "FtRange");

            List<Setup30_DigTypes> lstDigTypes = new List<Setup30_DigTypes>();
            foreach (var record in ds.Tables[3].AsEnumerable())
            {
                Setup30_DigTypes source = new Setup30_DigTypes();
                source.DigTypeID = Convert.ToInt32(record["DigTypeID"]);
                source.DigType = Convert.ToString(record["DigType"]);
                lstDigTypes.Add(source);
            }
            ViewBag.DigTypes = new SelectList(lstDigTypes, "DigTypeID", "DigType");

            List<Setup96_TaxCalcTypes> lstTaxCalc = new List<Setup96_TaxCalcTypes>();
            foreach (var record in ds.Tables[4].AsEnumerable())
            {
                Setup96_TaxCalcTypes source = new Setup96_TaxCalcTypes();
                source.TaxCalcTypeID = Convert.ToInt32(record["TaxCalcTypeID"]);
                source.TaxCalcType = Convert.ToString(record["TaxCalcType"]);
                lstTaxCalc.Add(source);
            }
            ViewBag.TaxCalcTypes = new SelectList(lstTaxCalc, "TaxCalcTypeID", "TaxCalcType");

            List<INV07_UnitOfMeasure> lstUOM = new List<INV07_UnitOfMeasure>();
            foreach (var record in ds.Tables[5].AsEnumerable())
            {
                INV07_UnitOfMeasure source = new INV07_UnitOfMeasure();
                source.UnitOfMeasure = Convert.ToString(record["UnitOfMeasure"]);
                source.UomDescription = Convert.ToString(record["UomDescription"]);
                lstUOM.Add(source);
            }
            ViewBag.UOMTypes = new SelectList(lstUOM, "UnitOfMeasure", "UomDescription");

            List<BID10A_BiTypes> lstBiTypes = new List<BID10A_BiTypes>();
            foreach (var record in ds.Tables[6].AsEnumerable())
            {
                BID10A_BiTypes source = new BID10A_BiTypes();
                source.BITypeId = Convert.ToInt32(record["BITypeId"]);
                source.BITypeName = Convert.ToString(record["BITypeName"]);
                lstBiTypes.Add(source);
            }
            ViewBag.BiTypes = new SelectList(lstBiTypes, "BITypeId", "BITypeName");

            List<RelatedBIDS> lstRelatedBids = new List<RelatedBIDS>();
            foreach (var record in ds.Tables[7].AsEnumerable())
            {
                lstRelatedBids.Add(new RelatedBIDS { BIDID = Convert.ToInt32(record["BIDID"]) });
            }
            ViewBag.RelatedBIs = new SelectList(lstRelatedBids, "BIDID", "BIDID");
        }

        [HttpPost]
        public ActionResult saveStyleHeight(BID01_Headers header)
        {
            int BIDID, BIDMATID = 0;
            header.BIDStatusID = Convert.ToString(Session["JobStatusId"]);
            header.InRollup = 1;
            header.SalTxPer = 0;
            header.EditBidItemFlag = 0;
            header.PercentOfHtStd = 0;
            header.PercentOfFtRangeStd = 0;
            header.PercentOfDigStandard = 0;
            header.SupervisonMarkup = 0;
            header.MaterialMarkUp = 0;
            header.LaborMarkUp = 0;
            header.JobMarkUp = 0;
            header.PFPJobMarkup = 0;
            header.PRJID = Convert.ToInt32(Session["PRJID"]);

            var result = ProjectDAL.saveStyleHeight(header);
            BIDID = result;
            if (BIDID > 0)
            {
                header.BIDID = BIDID;
                var matHeader = new BID03_MaterialHeader();
                matHeader.BIDMatHeaderID = header.BIDMatHeaderID;
                if (header.CFSFiles[0] != null)
                {

                    matHeader.CfsFileName = header.CFSFiles[0].FileName;
                    matHeader.OverRiddenCost = 0;
                    matHeader.BIDID = BIDID;
                    matHeader.EmployeeID = Convert.ToString(Session["CommID"]);
                    if (header.OverrideCost)
                    {
                        matHeader.OverRideCost = Convert.ToByte(header.OverrideCost);
                        matHeader.OverRiddenCost = Convert.ToDecimal(header.MaterialCost);
                    }
                    matHeader.BIDMatHeaderID = 0;
                    CFSProcessingDAL.DeleteExistingCFS(header.BIDID, 1);
                    header.BIDMatHeaderID = 0;
                    BIDMATID = ProjectDAL.saveMaterialHeader(matHeader);
                    header.BIDMatHeaderID = BIDMATID;
                    if (BIDMATID > 0)
                    {

                        string cfsDatapath = Server.MapPath("~/App_Data/CFS.xml");
                        //Server.MapPath(ConfigurationManager.AppSettings["menuListPathConfig"].ToString());
                        result = CFSProcessingDAL.saveCFSFile(header.CFSFiles[0], BIDMATID, header.OverrideCost, matHeader.BIDID.ToString(), cfsDatapath);
                    }
                }
                else
                {
                    matHeader.BIDID = BIDID;
                    matHeader.EmployeeID = Convert.ToString(Session["CommID"]);
                    matHeader.CfsFileName = header.CFSFileName;
                    matHeader.OverRideCost = Convert.ToByte(header.OverrideCost);
                    matHeader.OverRiddenCost = Convert.ToDecimal(header.MaterialCost);
                    BIDMATID = ProjectDAL.saveMaterialHeader(matHeader);
                    Session["BIDMATID"] = BIDMATID;
                }
            }

            if (result > 0)
            {
                Session["FenceTypeID"] = header.FenceTypeID;
                Session["BidId"] = header.BIDID;
                return RedirectToAction("viewmakeitems", new { BIDMATID = BIDMATID });
            }
            else
            {
                LoadComboForHeight();
                ModelState.AddModelError(String.Empty, BusinessConstants.contactAdmin);
                return View("detail/selectheight", header);
            }
        }
        [Route("project/labourdetails")]
        [Route("project/labourdetails/edit/{id}")]
        public ActionResult LabourDetails(string fence, string bid, string id = "0")
        {

            int editBid = Int32.Parse(id);
            BILabour BiLabour = new BILabour();
            bid = Convert.ToString(Session["BidId"]);
            fence = Convert.ToString(Session["FenceTypeID"]);
            //fence = "23";
            //bid = "4062";
            if (string.IsNullOrWhiteSpace(bid) || string.IsNullOrWhiteSpace(fence))
                return View("detail/selectheight");
            ViewBag.FenceType = Convert.ToInt32(fence);
            ViewBag.BIDID = Convert.ToInt32(bid);

            //Load Main Labour Section
            List<LabourDetails> labourList = new List<Models.DBO.LabourDetails>();
            List<LabourLabels> labourLabels = ProjectDAL.getLabourLabels(Convert.ToString(ViewBag.FenceType));

            var LabourByBid = ProjectDAL.getLabourDetailsByBid(bid);

            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[0].Rows.Count > 0)
            {
                foreach (var item in labourLabels)
                {
                    List<LabourDdls> labourDdls = new List<LabourDdls>();
                    if (item.ElemLabelText.ToLower().Contains("dig"))
                    {
                        labourDdls = ProjectDAL.getLabourDetails(null, item.FieldLbrTypeID, bid);
                    }
                    else
                    {
                        labourDdls = ProjectDAL.getLabourDetails(Convert.ToString(ViewBag.FenceType), item.FieldLbrTypeID, bid);
                    }
                    var selectedData = LabourByBid.Tables[0].AsEnumerable().Where(i => i.Field<Int32>("FieldLbrTypeID") == item.FieldLbrTypeID);
                    if (selectedData.Count() > 0)
                    {
                        foreach (DataRow labour in selectedData)
                        {
                            LabourDetails labourDetails = new Models.DBO.LabourDetails();
                            int selectedLabour = labour != null ? labour.Field<int>("FieldLbrDtlsID") : 0;
                            labourDetails.UOM = labour != null ? labour.Field<string>("FieldLbrUom") : string.Empty;
                            labourDetails.Quantity = Convert.ToInt32(labour != null ? labour.Field<decimal>("Qty") : 0);
                            labourDetails.labourDdlForUI = new SelectList(labourDdls, "FieldLbrDtlsID", "FieldLbrDesc", selectedLabour);
                            labourDetails.BIDID = labour != null ? labour.Field<int>("BIDID") : ViewBag.BIDID;
                            labourDetails.labourLabel = item.ElemLabelText;
                            labourDetails.BIDLaborItemsID = labour != null ? labour.Field<int>("BIDLaborItemsID") : 0;
                            labourList.Add(labourDetails);
                        }
                    }
                    else
                    {
                        LabourDetails labourDetails = new Models.DBO.LabourDetails();
                        labourDetails.labourDdlForUI = new SelectList(labourDdls, "FieldLbrDtlsID", "FieldLbrDesc");
                        labourDetails.BIDID = ViewBag.BIDID;
                        labourDetails.labourLabel = item.ElemLabelText;
                        labourList.Add(labourDetails);
                    }

                }
            }
            else
            {
                foreach (var item in labourLabels)
                {
                    List<LabourDdls> labourDdls = new List<LabourDdls>();
                    if (item.ElemLabelText.ToLower().Contains("dig"))
                    {
                        labourDdls = ProjectDAL.getLabourDetails(null, item.FieldLbrTypeID, bid);
                    }
                    else
                    {
                        labourDdls = ProjectDAL.getLabourDetails(Convert.ToString(ViewBag.FenceType), item.FieldLbrTypeID, bid);
                    }
                    LabourDetails labourDetails = new Models.DBO.LabourDetails();
                    labourDetails.BIDID = ViewBag.BIDID;
                    labourDetails.labourLabel = item.ElemLabelText;
                    labourDetails.labourDdlForUI = new SelectList(labourDdls, "FieldLbrDtlsID", "FieldLbrDesc");
                    labourList.Add(labourDetails);
                }
            }
            BiLabour.LabourDetails = labourList;

            //Load for all dropdowns in Labour except MainLabour
            DataSet labourCombos = ProjectDAL.getComboForLabourSections(bid);

            List<LabourDetails> addnList = new List<Models.DBO.LabourDetails>();
            List<LabourDdls> additionalDropdown = ComboCollection.AdditionalLabourCombo(labourCombos.Tables[0]);

            var mainlabourids = LabourByBid.Tables[0].AsEnumerable().Select(i => i.Field<Int32>("FieldLbrTypeID")).ToArray();
            var additionalLabourids = LabourByBid.Tables[0].AsEnumerable().Select(i => i.Field<int>("FieldLbrTypeID")).Except(mainlabourids);
            //Extras for Additional Labour
            int extraLabour = 5;
            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[0].Rows.Count > 0)
            {
                //foreach (var ids in additionalLabourids)
                //{
                var selectedData = LabourByBid.Tables[0].AsEnumerable().Where(i => i.Field<Int32>("FieldLbrTypeID") == 24);
                if (selectedData.Count() > 0)
                {
                    foreach (DataRow labour in selectedData)
                    {
                        LabourDetails labourDetails = new Models.DBO.LabourDetails();
                        int selectedLabour = labour != null ? labour.Field<int>("FieldLbrDtlsID") : 0;
                        labourDetails.UOM = labour != null ? labour.Field<string>("FieldLbrUom") : string.Empty;
                        labourDetails.Quantity = Convert.ToInt32(labour != null ? labour.Field<decimal>("Qty") : 0);
                        labourDetails.labourDdlForUI = new SelectList(additionalDropdown, "FieldLbrDtlsID", "FieldLbrDesc", labour.Field<int>("FieldLbrDtlsID"));
                        labourDetails.BIDID = labour != null ? labour.Field<int>("BIDID") : ViewBag.BIDID;
                        labourDetails.labourLabel = labourDetails.labourLabel = "Extras";
                        labourDetails.BIDLaborItemsID = labour != null ? labour.Field<int>("BIDLaborItemsID") : 0;
                        addnList.Add(labourDetails);
                    }
                }
                //}
            }
            extraLabour = extraLabour - addnList.Count;
            for (int i = 0; i < extraLabour; i++)
            {
                LabourDetails labourDetails = new Models.DBO.LabourDetails();
                labourDetails.BIDID = ViewBag.BIDID;
                labourDetails.labourLabel = "Extras";
                labourDetails.labourDdlForUI = new SelectList(additionalDropdown, "FieldLbrDtlsID", "FieldLbrDesc");
                addnList.Add(labourDetails);
            }
            BiLabour.AddlnLabourDetails = addnList;

            BiLabour.ReviewLabourDetails = new List<ReviewLabour>();
            if (LabourByBid.Tables.Count > 4 && LabourByBid.Tables[5].Rows.Count > 0)
            {
                foreach (var item in LabourByBid.Tables[5].AsEnumerable())
                {
                    var source = new ReviewLabour();
                    source.BIDID = Convert.ToString(item["BIDID"]);
                    source.FieldLbrDtlsID = Convert.ToString(item["FieldLbrDtlsID"]);
                    source.FieldLbrDesc = Convert.ToString(item["FieldLbrDesc"]);
                    source.Qty = Convert.ToInt32(item["Qty"]);
                    source.MhsEa = Convert.ToDecimal(item["MhsEa"]);
                    source.MhsTotal = Convert.ToDecimal(item["MhsTotal"]);
                    source.CrewPayEa = Convert.ToDecimal(item["CrewPayEa"]);
                    source.CrewPayTotal = Convert.ToDecimal(item["CrewPayTotal"]);
                    source.SubPayEa = Convert.ToDecimal(item["SubPayEa"]);
                    source.SubPayTotal = Convert.ToDecimal(item["SubPayTotal"]);
                    BiLabour.ReviewLabourDetails.Add(source);
                }
            }
            //Load for all combos
            var postTypes = ComboCollection.PostTypesCombo(labourCombos.Tables[1]);
            var concreteType = ComboCollection.ConcreteTypesCombo(labourCombos.Tables[2]);
            var roundFactors = ComboCollection.RoundFactorCombo();
            var othercost = ComboCollection.OtherCostCombo(labourCombos.Tables[3]);
            var crewsize = ComboCollection.CrewSizeCombo(labourCombos.Tables[4]);
            var equipcost = ComboCollection.EquipmentCostsCombos(labourCombos.Tables[5]);

            BiLabour.ConcreteDetails = new List<BID07_Concrete>();
            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[1].Rows.Count > 0)
            {
                foreach (var item in LabourByBid.Tables[1].AsEnumerable())
                {
                    var source = new BID07_Concrete();
                    source.BIDID = Convert.ToInt32(item["BIDID"]);
                    source.CONCID = Convert.ToInt32(item["CONCID"]);
                    source.PstTypId = Convert.ToString(item["PstTypId"]);
                    source.ConcreteType = Convert.ToString(item["ConcreteType"]);
                    source.HoleQty = Convert.ToInt32(item["HoleQty"]);
                    source.HoleDia = Convert.ToInt32(item["HoleDia"]);
                    source.HoleDepth = Convert.ToInt32(item["HoleDepth"]);
                    source.RoundFactor = Convert.ToInt32(item["RoundFactor"]);
                    source.PostTypeDdlForUI = new SelectList(postTypes, "PstTypID", "PostType", source.PstTypId);
                    source.ConcreteTypeDdlForUI = new SelectList(concreteType, "PartNum", "PartDescription", source.ConcreteType);
                    source.RoundFactorDdlForUI = new SelectList(roundFactors, "Value", "Text", source.RoundFactor);
                    BiLabour.ConcreteDetails.Add(source);
                }
            }
            else
            {
                BiLabour.ConcreteDetails.Add(new BID07_Concrete
                {
                    BIDID = ViewBag.BIDID,
                    PostTypeDdlForUI = new SelectList(postTypes, "PstTypID", "PostType"),
                    ConcreteTypeDdlForUI = new SelectList(concreteType, "PartNum", "PartDescription"),
                    RoundFactorDdlForUI = new SelectList(roundFactors, "Value", "Text")
                });

            }

            BiLabour.BidOtherCosts = new List<BID08_OtherCosts>();
            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[2].Rows.Count > 0)
            {
                foreach (var item in LabourByBid.Tables[2].AsEnumerable())
                {
                    var source = new BID08_OtherCosts
                    {
                        BIDID = Convert.ToInt32(item["BIDID"]),
                        OtherCostID = Convert.ToInt32(item["OtherCostID"]),
                        OtherCostTypeID = Convert.ToInt32(item["OtherCostTypeID"]),
                        Qty = Convert.ToDecimal(item["Qty"]),
                        Cost = Convert.ToDecimal(item["Cost"]),
                        OtherCostTypeDdlForUI = new SelectList(othercost, "OtherCostTypeID", "OtherCostType", Convert.ToInt32(item["OtherCostTypeID"]))
                    };
                    BiLabour.BidOtherCosts.Add(source);
                }
            }
            else
            {
                BiLabour.BidOtherCosts.Add(new BID08_OtherCosts
                {
                    BIDID = ViewBag.BIDID,
                    OtherCostTypeDdlForUI = new SelectList(othercost, "OtherCostTypeID", "OtherCostType")
                });
            }

            BiLabour.BidCrewProfile = new List<BID06_CrewProfile>();
            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[3].Rows.Count > 0)
            {
                foreach (var item in LabourByBid.Tables[3].AsEnumerable())
                {
                    var source = new BID06_CrewProfile
                    {
                        BIDID = Convert.ToInt32(item["BIDID"]),
                        BIDDefineCrewID = Convert.ToInt32(item["BIDDefineCrewID"]),
                        CrewPositionID = Convert.ToInt32(item["CrewPositionID"]),
                        HeadCount = Convert.ToInt32(item["HeadCount"]),
                        CrewPositionDdlForUI = new SelectList(crewsize, "CrewPositionID", "CrewPosition", Convert.ToInt32(item["CrewPositionID"]))
                    };
                    BiLabour.BidCrewProfile.Add(source);
                }
            }
            else
            {
                BiLabour.BidCrewProfile.Add(new BID06_CrewProfile
                {
                    BIDID = ViewBag.BIDID,
                    CrewPositionDdlForUI = new SelectList(crewsize, "CrewPositionID", "CrewPosition")
                });
            }

            BiLabour.BidEquipmentBurden = new List<BID09_EquipmentBurden>();
            if (LabourByBid.Tables.Count > 0 && LabourByBid.Tables[4].Rows.Count > 0)
            {
                foreach (var item in LabourByBid.Tables[4].AsEnumerable())
                {
                    var source = new BID09_EquipmentBurden
                    {
                        BIDID = Convert.ToInt32(item["BIDID"]),
                        EquipBurdID = Convert.ToInt32(item["EquipBurdID"]),
                        EquipCostID = Convert.ToInt32(item["EquipCostID"]),
                        Qty = Convert.ToInt32(item["Qty"]),
                        EquipCostDdlForUI = new SelectList(equipcost, "EquipCostID", "EquipName", Convert.ToInt32(item["EquipCostID"]))
                    };
                    BiLabour.BidEquipmentBurden.Add(source);
                }
            }
            else
            {
                BiLabour.BidEquipmentBurden.Add(new BID09_EquipmentBurden
                {
                    BIDID = ViewBag.BIDID,
                    EquipCostDdlForUI = new SelectList(equipcost, "EquipCostID", "EquipName")
                });
            }

            return View(BiLabour);
        }

        [HttpPost]
        public JsonResult saveMainLabor(List<LabourDetails> labourDetails)
        {
            int result = -2;
            {
                labourDetails = ProjectDAL.saveMainLabour(labourDetails);
            }
            return Json(labourDetails);
        }

        [HttpPost]
        public JsonResult saveConcrete(List<BID07_Concrete> ConcreteDetails)
        {
            int result = -2;
            {
                ConcreteDetails = ProjectDAL.saveConcrete(ConcreteDetails);
            }
            return Json(ConcreteDetails);
        }

        [HttpPost]
        public JsonResult saveotherCosts(List<BID08_OtherCosts> OtherCostsDetails)
        {
            int result = -2;
            {
                OtherCostsDetails = ProjectDAL.saveOtherCostsDetails(OtherCostsDetails);
            }
            return Json(OtherCostsDetails);
        }

        [HttpPost]
        public JsonResult saveCrewSize(List<BID06_CrewProfile> CrewProfile)
        {
            int result = -2;
            {
                CrewProfile = ProjectDAL.saveCrewProfile(CrewProfile);
            }
            return Json(CrewProfile);
        }

        [HttpPost]
        public JsonResult saveEquipments(List<BID09_EquipmentBurden> EquipmentBurden)
        {
            int result = -2;
            {
                EquipmentBurden = ProjectDAL.saveEquipmentBurden(EquipmentBurden);
            }
            return Json(EquipmentBurden);
        }

        [HttpPost]
        public JsonResult RemoveRecord(int id, string section, string idName)
        {
            int result = -2;
            {
                result = ProjectDAL.RemoveLabourSectionRow(id, section, idName);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetUOMDesc(string id)
        {
            string uom = "";
            {
                uom = ProjectDAL.getUOMDesc(id);
            }
            return Json(uom);
        }

        [Route("project/viewbid")]
        [Route("project/viewbid/proj/{PRJID}")]
        public ActionResult ViewBid(string PRJID = "")
        {
            if (!string.IsNullOrWhiteSpace(PRJID))
            {
                Session["PRJID"] = PRJID;
            }
            if (!string.IsNullOrWhiteSpace((string)Session["CommID"]) && Session["PRJID"] != null && Convert.ToInt32(Session["PRJID"]) > 0)
            {
                ViewBag.Title = string.Format("{0}", Session["PRJID"]);
                var id = Convert.ToInt32(Session["PRJID"]);
                var model = new List<BidItems>();
                if (id > 0)
                {
                    model = ProjectDAL.GetBidItemsByProject(id);
                }

                return View(model);
            }
            else
            {
                return Redirect("index");
            }
        }

        [Route("Project/JobActivation/Checklist/{PRJID}")]
        public ActionResult JobChecklist(string PRJID)
        {
            if (!string.IsNullOrWhiteSpace(PRJID))
            {
                Session["PRJID"] = PRJID;
            }
            if (!string.IsNullOrWhiteSpace((string)Session["CommID"]) && Session["PRJID"] != null && Convert.ToInt32(Session["PRJID"]) > 0)
            {
                ViewBag.Title = string.Format("{0}", Session["PRJID"]);
                var id = Convert.ToInt32(Session["PRJID"]);
                var model = new JobActivationChecklistModel();
                if (id > 0)
                {
                    model.PRJID = id;
                    model = ProjectDAL.getProjectActivationDetails(model);
                    LoadActivationLookup(model);
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

       [HttpPost]
        public ActionResult SaveActivation(JobActivationChecklistModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = ProjectDAL.saveJobActivation(model);
                    if (result > 0)
                    {
                            return RedirectToAction("index", "home");
                    }
                }
                LoadActivationLookup(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message + BusinessConstants.contactAdmin);
                LoadActivationLookup(model);
                return View(model);
            }
        }

        public void LoadActivationLookup(JobActivationChecklistModel model)
        {
            model = ProjectDAL.JobActivationLookup(model);
            IEnumerable<SelectListItem> ListCustomersType = model.ListCustomersType.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.CustomerTypeId),
                Text = c.Description

            });
            ViewBag.ListCustomersType = ListCustomersType;

            IEnumerable<SelectListItem> ListJobType = model.ListJobTypes.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.JobTypeId),
                Text = c.JobTypeDesc

            });
            ViewBag.ListJobType = ListJobType;

            IEnumerable<SelectListItem> ListResponses = model.ListResponses.Select(c => new SelectListItem
            {
                Value = Convert.ToString(c.ResponseId),
                Text = c.Response

            });
            ViewBag.ListResponses = ListResponses;
        }


    }
}


